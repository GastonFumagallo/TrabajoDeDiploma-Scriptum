/* ============================================================================================
   SCRIPTUM — Datos de prueba para capturas de pantalla (Entrega Final)
   --------------------------------------------------------------------------------------------
   Qué hace:
     1. Vacía SOLO las tablas de negocio (ventas, cajas, stock, órdenes, libros, géneros,
        clientes, proveedores) y las Personas que no pertenecen a ningún usuario.
     2. Carga géneros, libros, proveedores, clientes, 3 cajas (2 cerradas y 1 abierta hoy),
        16 ventas (una anulada), 3 órdenes de reposición y el historial de stock (kardex).

   Qué NO toca:
     - Seguridad: Usuarios, Grupos, Acciones, Formularios, Módulos, estados, relaciones
       usuario-grupo/acción, auditorías de sesión y de seguridad.
     - Personas de los usuarios del sistema.
     - Medios de pago: se conservan; solo se agregan Efectivo, Tarjeta de débito,
       Tarjeta de crédito o Transferencia si faltara alguno.

   Requisitos: al menos un usuario activo (las cajas se abren y cierran a su nombre).
   Todo corre en una transacción: si algo falla, no queda ningún cambio.
   ============================================================================================ */

-- Opciones que exigen los índices filtrados del esquema (SSMS ya las trae así; sqlcmd no).
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* ---------------------------------------------------------------------------------------
       0. Usuarios que operan las cajas (se toman de los existentes, no se modifican)
       --------------------------------------------------------------------------------------- */
    DECLARE @uA int, @uB int, @nA nvarchar(100), @nB nvarchar(100);

    SELECT TOP (1) @uA = u.USU_ID, @nA = u.USU_Nombre
    FROM Usuarios u LEFT JOIN Estados_Usuarios e ON e.EST_USU_ID = u.EST_USU_ID
    WHERE ISNULL(e.EST_USU_Nombre, N'Activo') = N'Activo'
    ORDER BY u.USU_ID;

    IF @uA IS NULL
        THROW 50001, N'No hay ningún usuario activo: las cajas necesitan un usuario que las abra.', 1;

    SELECT TOP (1) @uB = u.USU_ID, @nB = u.USU_Nombre
    FROM Usuarios u LEFT JOIN Estados_Usuarios e ON e.EST_USU_ID = u.EST_USU_ID
    WHERE ISNULL(e.EST_USU_Nombre, N'Activo') = N'Activo' AND u.USU_ID <> @uA
    ORDER BY u.USU_ID;

    IF @uB IS NULL SELECT @uB = @uA, @nB = @nA;   -- con un solo usuario, opera las tres cajas

    /* ---------------------------------------------------------------------------------------
       1. Limpieza de tablas de negocio (de hijas a padres)
       --------------------------------------------------------------------------------------- */
    DELETE FROM CajaTotalesMedio;
    DELETE FROM PagosVenta;
    DELETE FROM DetallesVenta;
    DELETE FROM Ventas;
    DELETE FROM Cajas;
    DELETE FROM MovimientosStock;
    DELETE FROM DetallesOrdenReposicion;
    DELETE FROM OrdenesReposicion;
    DELETE FROM ProveedoresLibros;
    DELETE FROM Libros;
    DELETE FROM Generos;
    DELETE FROM Clientes;
    DELETE FROM Proveedores;
    DELETE FROM Personas
    WHERE PER_ID NOT IN (SELECT PER_ID FROM Usuarios WHERE PER_ID IS NOT NULL);

    -- Reinicio de IDENTITY solo en las tablas vaciadas por completo (Personas conserva usuarios).
    -- Una tabla que nunca tuvo filas no se reinicia: con RESEED 0 su primer Id sería 0 y no 1.
    DECLARE @tabla sysname;
    DECLARE c CURSOR LOCAL FAST_FORWARD FOR
        SELECT t FROM (VALUES (N'CajaTotalesMedio'), (N'PagosVenta'), (N'DetallesVenta'), (N'Ventas'), (N'Cajas'),
                              (N'MovimientosStock'), (N'DetallesOrdenReposicion'), (N'OrdenesReposicion'),
                              (N'Libros'), (N'Generos'), (N'Clientes'), (N'Proveedores')) AS x(t);
    OPEN c;
    FETCH NEXT FROM c INTO @tabla;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(@tabla) AND last_value IS NOT NULL)
            DBCC CHECKIDENT (@tabla, RESEED, 0) WITH NO_INFOMSGS;
        FETCH NEXT FROM c INTO @tabla;
    END
    CLOSE c;
    DEALLOCATE c;

    /* ---------------------------------------------------------------------------------------
       2. Medios de pago (se conservan; se agregan los que falten)
          El ajuste replica las estrategias del sistema: crédito +10 %, débito −5 %, transferencia −2 %.
       --------------------------------------------------------------------------------------- */
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE UPPER(LTRIM(RTRIM(MP_Nombre))) = N'EFECTIVO')
        INSERT INTO MetodosPago (MP_Nombre, MP_Estado) VALUES (N'Efectivo', 1);
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE UPPER(LTRIM(RTRIM(MP_Nombre))) IN (N'TARJETA DE DEBITO', N'TARJETA DE DÉBITO'))
        INSERT INTO MetodosPago (MP_Nombre, MP_Estado) VALUES (N'Tarjeta de débito', 1);
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE UPPER(LTRIM(RTRIM(MP_Nombre))) IN (N'TARJETA DE CREDITO', N'TARJETA DE CRÉDITO'))
        INSERT INTO MetodosPago (MP_Nombre, MP_Estado) VALUES (N'Tarjeta de crédito', 1);
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE UPPER(LTRIM(RTRIM(MP_Nombre))) = N'TRANSFERENCIA')
        INSERT INTO MetodosPago (MP_Nombre, MP_Estado) VALUES (N'Transferencia', 1);

    DECLARE @mp TABLE (k char(2) PRIMARY KEY, MP_ID int NOT NULL, Factor decimal(5,2) NOT NULL, EsEfectivo bit NOT NULL);
    INSERT INTO @mp
    SELECT k, MIN(MP_ID), Factor, EsEfectivo
    FROM (SELECT MP_ID,
                 CASE UPPER(LTRIM(RTRIM(MP_Nombre)))
                      WHEN N'EFECTIVO' THEN 'EF' WHEN N'TRANSFERENCIA' THEN 'TR'
                      WHEN N'TARJETA DE DEBITO' THEN 'DE' WHEN N'TARJETA DE DÉBITO' THEN 'DE'
                      WHEN N'TARJETA DE CREDITO' THEN 'CR' WHEN N'TARJETA DE CRÉDITO' THEN 'CR' END AS k
          FROM MetodosPago) m
    JOIN (VALUES ('EF', 1.00, 1), ('DE', 0.95, 0), ('CR', 1.10, 0), ('TR', 0.98, 0)) f(fk, Factor, EsEfectivo) ON f.fk = m.k
    GROUP BY k, Factor, EsEfectivo;

    /* ---------------------------------------------------------------------------------------
       3. Géneros (categorías)
       --------------------------------------------------------------------------------------- */
    SET IDENTITY_INSERT Generos ON;
    INSERT INTO Generos (GEN_ID, GEN_Nombre, GEN_Descripcion) VALUES
        (1, N'Ficción y Literatura',         N'Novela y cuento de autores argentinos, latinoamericanos y universales.'),
        (2, N'Ciencia Ficción y Fantasía',   N'Distopías, sagas fantásticas y ciencia ficción clásica y contemporánea.'),
        (3, N'Historia y Política',          N'Ensayo histórico, crónica periodística y divulgación de ciencias sociales.'),
        (4, N'Desarrollo Personal',          N'Hábitos, psicología, bienestar y crecimiento personal.'),
        (5, N'Infantil y Juvenil',           N'Lecturas para chicos y adolescentes.'),
        (6, N'Historieta y Novela Gráfica',  N'Cómic, historieta argentina y novela gráfica.');
    SET IDENTITY_INSERT Generos OFF;

    /* ---------------------------------------------------------------------------------------
       4. Libros (stock final; el inicial se reconstruye en el kardex del paso 11)
          Estados del inventario: stock ≤ mínimo = Crítico; stock ≤ punto de reposición = A reponer.
       --------------------------------------------------------------------------------------- */
    SET IDENTITY_INSERT Libros ON;
    INSERT INTO Libros (LIB_ID, LIB_ISBN, LIB_Titulo, LIB_Autor, LIB_Editorial, LIB_AñoPublicacion, GEN_ID,
                        LIB_PrecioVenta, LIB_PrecioCosto, LIB_Stock, LIB_StockMinimo, LIB_PuntoReposicion, LIB_StockOptimo,
                        LIB_Descripcion, LIB_Activo, LIB_Version) VALUES
        ( 1, N'9789500723459', N'Rayuela', N'Julio Cortázar', N'Sudamericana', 1963, 1, 32900, 19700, 24, 3,  8, 30,
             N'Novela emblemática del boom latinoamericano, que puede leerse en más de un orden.', 1, 0),
        ( 2, N'9789504912347', N'Ficciones', N'Jorge Luis Borges', N'Penguin Random House', 1944, 1, 24500, 14700, 18, 3,  8, 25,
             N'Cuentos fundamentales de Borges, entre ellos "Tlön, Uqbar, Orbis Tertius" y "La biblioteca de Babel".', 1, 0),
        ( 3, N'9789500730013', N'Cien años de soledad', N'Gabriel García Márquez', N'Sudamericana', 1967, 1, 35900, 21500, 27, 3,  8, 30,
             N'La saga de la familia Buendía en Macondo.', 1, 0),
        ( 4, N'9789877384567', N'Nuestra parte de noche', N'Mariana Enriquez', N'Anagrama', 2019, 1, 42500, 25500, 16, 3,  8, 20,
             N'Premio Herralde de Novela. Terror, herencia y dictadura en la Argentina.', 1, 0),
        ( 5, N'9789876281232', N'El Eternauta', N'Héctor G. Oesterheld y Francisco Solano López', N'Doedytores', 1957, 6, 38000, 22800,  1, 3,  8, 25,
             N'Clásico de la historieta argentina: una nevada mortal cae sobre Buenos Aires.', 1, 0),
        ( 6, N'9789500822107', N'Dune', N'Frank Herbert', N'Penguin Random House', 1965, 2, 39900, 23900,  2, 3,  8, 25,
             N'La épica de Paul Atreides en el planeta desierto Arrakis.', 1, 0),
        ( 7, N'9788445087657', N'El Señor de los Anillos: La Comunidad del Anillo', N'J. R. R. Tolkien', N'Minotauro', 1954, 2, 44900, 26900, 19, 3,  8, 25,
             N'Primera parte de la trilogía de la Tierra Media.', 1, 0),
        ( 8, N'9789507312342', N'1984', N'George Orwell', N'Penguin Random House', 1949, 2, 21900, 13100, 35, 5, 12, 40,
             N'La distopía del Gran Hermano y la vigilancia total.', 1, 0),
        ( 9, N'9789500445672', N'Fahrenheit 451', N'Ray Bradbury', N'Minotauro', 1953, 2, 23500, 14100, 22, 3,  8, 25,
             N'Un futuro donde los bomberos queman libros.', 1, 0),
        (10, N'9789875809987', N'Sapiens. De animales a dioses', N'Yuval Noah Harari', N'Penguin Random House', 2011, 3, 36500, 21900,  6, 3,  8, 25,
             N'Breve historia de la humanidad, de la revolución cognitiva a la actualidad.', 1, 0),
        (11, N'9789874005557', N'Operación masacre', N'Rodolfo Walsh', N'Ediciones de la Flor', 1957, 3, 19800, 11900, 21, 3,  8, 25,
             N'Investigación periodística sobre los fusilamientos de José León Suárez.', 1, 0),
        (12, N'9789505154326', N'Hábitos atómicos', N'James Clear', N'Planeta', 2018, 4, 29900, 17900,  1, 3,  8, 25,
             N'Cambios pequeños, resultados extraordinarios.', 1, 0),
        (13, N'9788498388886', N'El hombre en busca de sentido', N'Viktor E. Frankl', N'Herder', 1946, 4, 22500, 13500, 17, 3,  8, 25,
             N'Testimonio de los campos de concentración y bases de la logoterapia.', 1, 0),
        (14, N'9789873954122', N'El Principito', N'Antoine de Saint-Exupéry', N'Salamandra', 1943, 5, 16900, 10100, 38, 5, 12, 40,
             N'Clásico universal para todas las edades.', 1, 0),
        (15, N'9789504918905', N'Harry Potter y la piedra filosofal', N'J. K. Rowling', N'Salamandra', 1997, 5, 31500, 18900, 26, 3,  8, 30,
             N'Primer libro de la saga del joven mago.', 1, 0);
    SET IDENTITY_INSERT Libros OFF;

    /* ---------------------------------------------------------------------------------------
       5. Personas de clientes y proveedores
          Clientes: PER_DNI = DNI (o los 8 dígitos centrales del CUIT). Proveedores: PER_DNI = 0,
          como guarda el sistema (el proveedor se identifica por CUIT; la persona es el contacto).
       --------------------------------------------------------------------------------------- */
    DECLARE @per TABLE (k varchar(10) PRIMARY KEY, Nombre nvarchar(60), Mail nvarchar(100), Tel nvarchar(40), Dni int);
    INSERT INTO @per VALUES
        ('CF',  N'Consumidor Final',               N'',                                     N'',                0),
        ('C2',  N'María Laura Gómez',              N'mlgomez@gmail.com',                    N'341 155 482 913', 30456789),
        ('C3',  N'Juan Pablo Rodríguez',           N'jprodriguez@hotmail.com',              N'341 156 734 210', 27123456),
        ('C4',  N'Lucía Fernández',                N'lucia.fernandez@gmail.com',            N'341 153 908 274', 38765432),
        ('C5',  N'Diego Martín Sosa',              N'dsosa@yahoo.com.ar',                   N'341 157 112 045', 25987654),
        ('C6',  N'Carolina Benítez',               N'carobenitez@gmail.com',                N'341 152 641 778', 33221144),
        ('C7',  N'Sofía Acosta',                   N'sofi.acosta@outlook.com',              N'341 158 305 662', 41234567),
        ('C8',  N'Instituto Educativo del Sol',    N'administracion@institutodelsol.edu.ar', N'341 448 2210',   50421987),
        ('P1',  N'Martín Ferreyra',                N'ventas@distlitoral.com.ar',            N'341 425 6610',    0),
        ('P2',  N'Gabriela Ríos',                  N'pedidos@mayoristasantafe.com.ar',      N'342 455 1820',    0),
        ('P3',  N'Hernán Cabrera',                 N'compras@rosariolibros.com.ar',         N'341 421 9037',    0),
        ('P4',  N'Paula Medina',                   N'paula@edicionesdelparana.com.ar',      N'341 156 908 112', 0);

    DECLARE @perId TABLE (k varchar(10) PRIMARY KEY, PER_ID int NOT NULL);
    MERGE INTO Personas AS t
    USING @per AS s ON 1 = 0
    WHEN NOT MATCHED THEN
        INSERT (PER_Nombre, PER_Mail, PER_Telefono, PER_DNI) VALUES (s.Nombre, s.Mail, s.Tel, s.Dni)
    OUTPUT s.k, inserted.PER_ID INTO @perId (k, PER_ID);

    /* ---------------------------------------------------------------------------------------
       6. Clientes (el 1 es "Consumidor Final", cliente del sistema)
       --------------------------------------------------------------------------------------- */
    SET IDENTITY_INSERT Clientes ON;
    INSERT INTO Clientes (CLI_ID, PER_ID, CLI_TipoDocumento, CLI_Documento, CLI_Direccion, CLI_Localidad,
                          CLI_LimiteCredito, CLI_ConsumidorFinal, CLI_Activo, CLI_Version)
    SELECT v.id, p.PER_ID, v.tipo, v.doc, v.dir, v.loc, v.lim, v.cf, 1, 0
    FROM (VALUES
            (1, 'CF', N'DNI',  NULL,          NULL,                       NULL,                       0,      1),
            (2, 'C2', N'DNI',  N'30456789',   N'Bv. Oroño 1245',          N'Rosario',                 0,      0),
            (3, 'C3', N'DNI',  N'27123456',   N'Córdoba 2780',            N'Rosario',                 0,      0),
            (4, 'C4', N'DNI',  N'38765432',   N'Mendoza 1530',            N'Rosario',                 0,      0),
            (5, 'C5', N'DNI',  N'25987654',   N'Av. San Martín 4120',     N'Villa Gobernador Gálvez', 0,      0),
            (6, 'C6', N'DNI',  N'33221144',   N'Hipólito Yrigoyen 845',   N'Funes',                   0,      0),
            (7, 'C7', N'DNI',  N'41234567',   N'Rioja 3310',              N'Rosario',                 0,      0),
            (8, 'C8', N'CUIT', N'30504219875',N'Santa Fe 1850',           N'Rosario',                 250000, 0)
         ) v(id, k, tipo, doc, dir, loc, lim, cf)
    JOIN @perId p ON p.k = v.k;
    SET IDENTITY_INSERT Clientes OFF;

    /* ---------------------------------------------------------------------------------------
       7. Proveedores y libros que provee cada uno (con precio de compra)
       --------------------------------------------------------------------------------------- */
    SET IDENTITY_INSERT Proveedores ON;
    INSERT INTO Proveedores (PROV_ID, PER_ID, PROV_Empresa, PROV_CUIT, PROV_CondicionFiscal, PROV_Direccion,
                             PROV_Observaciones, PROV_Activo, PROV_Version)
    SELECT v.id, p.PER_ID, v.emp, v.cuit, v.cf, v.dir, v.obs, 1, 0
    FROM (VALUES
            (1, 'P1', N'Distribuidora del Litoral S.A.',      N'30712345612', N'Responsable Inscripto', N'Av. Pellegrini 1450, Rosario',
                N'Entrega en 48 h. Pedido mínimo $150.000.'),
            (2, 'P2', N'Librería Mayorista Santa Fe S.R.L.',  N'30709876526', N'Responsable Inscripto', N'Bv. Gálvez 2105, Santa Fe',
                N'Despacho por expreso los martes y viernes.'),
            (3, 'P3', N'Distribuidora Rosario Libros S.A.',   N'30715552236', N'Responsable Inscripto', N'Córdoba 1650, Rosario',
                N'Especialista en infantil y juvenil.'),
            (4, 'P4', N'Ediciones del Paraná',                N'33698765432', N'Monotributo',           N'San Martín 2310, Rosario',
                N'Editorial y distribuidora local de historieta y ensayo.')
         ) v(id, k, emp, cuit, cf, dir, obs)
    JOIN @perId p ON p.k = v.k;
    SET IDENTITY_INSERT Proveedores OFF;

    INSERT INTO ProveedoresLibros (PROV_ID, LIB_ID, PL_PrecioCompra)
    SELECT pl.prov, l.LIB_ID, ROUND(l.LIB_PrecioCosto * pl.factor, 2)
    FROM (VALUES (1, 1, 1.00), (1, 2, 1.00), (1, 3, 1.00), (1, 4, 1.00), (1, 10, 1.00), (1, 11, 1.03),
                 (2, 6, 1.00), (2, 7, 1.00), (2, 8, 1.00), (2, 9, 1.00), (2, 12, 1.00), (2, 13, 1.00),
                 (3, 2, 1.04), (3, 8, 1.03), (3, 14, 1.00), (3, 15, 1.00),
                 (4, 5, 1.00), (4, 11, 1.00)) pl(prov, lib, factor)
    JOIN Libros l ON l.LIB_ID = pl.lib;

    /* ---------------------------------------------------------------------------------------
       8. Cajas: 2 cerradas (anteayer y ayer) y 1 abierta hoy
          Los totales del arqueo se calculan en el paso 10, a partir de las ventas.
       --------------------------------------------------------------------------------------- */
    DECLARE @hoy  datetime2 = CAST(CAST(GETDATE() AS date) AS datetime2);
    DECLARE @b1   datetime2 = DATEADD(DAY, -2, @hoy);      -- anteayer 00:00
    DECLARE @b2   datetime2 = DATEADD(DAY, -1, @hoy);      -- ayer 00:00
    -- Caja de hoy: abre a las 9:00, o 4 horas antes de ahora si todavía no son las 13:00
    -- (así las ventas de hoy nunca quedan con hora futura).
    DECLARE @ap3  datetime2 = CASE WHEN DATEPART(HOUR, GETDATE()) >= 13 THEN DATEADD(HOUR, 9, @hoy)
                                   ELSE DATEADD(HOUR, -4, CAST(GETDATE() AS datetime2)) END;

    SET IDENTITY_INSERT Cajas ON;
    INSERT INTO Cajas (CAJ_ID, CAJ_Estado, CAJ_FechaApertura, CAJ_UsuarioAperturaId, CAJ_MontoInicial,
                       CAJ_FechaCierre, CAJ_UsuarioCierreId, CAJ_EfectivoContado, CAJ_Observaciones) VALUES
        (1, 1, DATEADD(MINUTE, 540, @b1), @uA, 20000, DATEADD(MINUTE, 1170, @b1), @uA, 0, NULL),
        (2, 1, DATEADD(MINUTE, 540, @b2), @uB, 25000, DATEADD(MINUTE, 1200, @b2), @uB, 0, N'Arqueo sin diferencias.'),
        (3, 0, @ap3,                      @uA, 30000, NULL,                       NULL, NULL, NULL);
    SET IDENTITY_INSERT Cajas OFF;

    DECLARE @cajero TABLE (CAJ_ID int PRIMARY KEY, Usuario nvarchar(100));
    INSERT INTO @cajero VALUES (1, @nA), (2, @nB), (3, @nA);

    /* ---------------------------------------------------------------------------------------
       9. Ventas, detalles y pagos
          Mismas fórmulas que CalculadoraVenta: Subtotal = Σ precio × cantidad; Descuento = % manual;
          Total = (Subtotal − Descuento) × ajuste del medio; IVA = 0 (tasa configurada en 0).
          En efectivo el cliente entrega el total redondeado a $1.000 y recibe vuelto.
       --------------------------------------------------------------------------------------- */
    DECLARE @v TABLE (n int PRIMARY KEY, Caja int, Fecha datetime2, Cli int, Mp char(2), Pct decimal(5,2),
                      Anulada bit, FechaAnul datetime2 NULL, MotivoAnul nvarchar(250) NULL);
    INSERT INTO @v VALUES
        -- Caja 1 (anteayer)
        ( 1, 1, DATEADD(MINUTE,  615, @b1), 2, 'EF',  0, 0, NULL, NULL),
        ( 2, 1, DATEADD(MINUTE,  700, @b1), 1, 'DE',  0, 0, NULL, NULL),
        ( 3, 1, DATEADD(MINUTE,  785, @b1), 8, 'TR', 10, 0, NULL, NULL),
        ( 4, 1, DATEADD(MINUTE,  980, @b1), 3, 'CR',  0, 0, NULL, NULL),
        ( 5, 1, DATEADD(MINUTE, 1065, @b1), 1, 'EF',  0, 1, DATEADD(MINUTE, 1075, @b1),
                N'El cliente desistió de la compra antes de retirar el libro.'),
        ( 6, 1, DATEADD(MINUTE, 1110, @b1), 4, 'EF',  0, 0, NULL, NULL),
        -- Caja 2 (ayer)
        ( 7, 2, DATEADD(MINUTE,  590, @b2), 5, 'CR',  0, 0, NULL, NULL),
        ( 8, 2, DATEADD(MINUTE,  670, @b2), 1, 'EF',  0, 0, NULL, NULL),
        ( 9, 2, DATEADD(MINUTE,  750, @b2), 6, 'DE',  0, 0, NULL, NULL),
        (10, 2, DATEADD(MINUTE,  945, @b2), 7, 'TR',  0, 0, NULL, NULL),
        (11, 2, DATEADD(MINUTE, 1040, @b2), 1, 'EF',  0, 0, NULL, NULL),
        (12, 2, DATEADD(MINUTE, 1140, @b2), 2, 'DE',  0, 0, NULL, NULL),
        -- Caja 3 (hoy, abierta)
        (13, 3, DATEADD(MINUTE,   40, @ap3), 1, 'EF', 0, 0, NULL, NULL),
        (14, 3, DATEADD(MINUTE,   90, @ap3), 3, 'DE', 0, 0, NULL, NULL),
        (15, 3, DATEADD(MINUTE,  140, @ap3), 8, 'TR', 5, 0, NULL, NULL),
        (16, 3, DATEADD(MINUTE,  190, @ap3), 6, 'CR', 0, 0, NULL, NULL);

    DECLARE @vd TABLE (n int, Lib int, Cant int, PRIMARY KEY (n, Lib));
    INSERT INTO @vd VALUES
        ( 1,  1, 1), ( 1, 14, 1),
        ( 2,  6, 1),
        ( 3, 15, 3), ( 3, 14, 2),
        ( 4, 10, 1), ( 4, 12, 1),
        ( 5,  2, 1),
        ( 6,  8, 1), ( 6,  9, 1),
        ( 7,  7, 1), ( 7,  6, 1),
        ( 8,  5, 2),
        ( 9,  4, 1),
        (10,  3, 1), (10, 11, 1),
        (11, 12, 2), (11, 13, 1),
        (12, 10, 1),
        (13,  1, 1), (13,  2, 1),
        (14,  8, 2),
        (15, 14, 5),
        (16, 15, 1), (16,  6, 1);

    DECLARE @calc TABLE (n int PRIMARY KEY, Subtotal decimal(18,2), Descuento decimal(18,2), Base decimal(18,2),
                         Total decimal(18,2), Recibido decimal(18,2), Vuelto decimal(18,2));
    INSERT INTO @calc (n, Subtotal, Descuento, Base, Total, Recibido, Vuelto)
    SELECT n, Subtotal, Descuento, Base, Total,
           -- El CAST dentro de cada rama es necesario: CEILING(...) * 1000 da decimal(38,0) y el CASE
           -- tomaría escala 0, redondeando los centavos de los pagos que no son en efectivo.
           CASE WHEN EsEfectivo = 1 THEN CAST(CEILING(Total / 1000) * 1000 AS decimal(18,2)) ELSE Total END,
           CASE WHEN EsEfectivo = 1 THEN CAST(CEILING(Total / 1000) * 1000 AS decimal(18,2)) - Total ELSE CAST(0 AS decimal(18,2)) END
    FROM (SELECT s.n, s.Subtotal, s.Descuento, s.Subtotal - s.Descuento AS Base,
                 ROUND((s.Subtotal - s.Descuento) * m.Factor, 2) AS Total, m.EsEfectivo
          FROM (SELECT v.n, v.Mp,
                       ROUND(SUM(l.LIB_PrecioVenta * d.Cant), 2) AS Subtotal,
                       ROUND(ROUND(SUM(l.LIB_PrecioVenta * d.Cant), 2) * v.Pct / 100, 2) AS Descuento
                FROM @v v JOIN @vd d ON d.n = v.n JOIN Libros l ON l.LIB_ID = d.Lib
                GROUP BY v.n, v.Mp, v.Pct) s
          JOIN @mp m ON m.k = s.Mp) x;

    SET IDENTITY_INSERT Ventas ON;
    INSERT INTO Ventas (VEN_ID, VEN_Fecha, CLI_ID, MP_ID, VEN_Subtotal, VEN_PorcentajeDescuento, VEN_Descuento,
                        VEN_AjusteMedioPago, VEN_IVA, VEN_Total, VEN_Usuario, CAJ_ID,
                        VEN_Anulada, VEN_FechaAnulacion, VEN_MotivoAnulacion, VEN_UsuarioAnulacion)
    SELECT v.n, v.Fecha, v.Cli, m.MP_ID, c.Subtotal, v.Pct, c.Descuento,
           c.Total - c.Base, 0, c.Total, cj.Usuario, v.Caja,
           v.Anulada, v.FechaAnul, v.MotivoAnul, CASE WHEN v.Anulada = 1 THEN cj.Usuario END
    FROM @v v JOIN @calc c ON c.n = v.n JOIN @mp m ON m.k = v.Mp JOIN @cajero cj ON cj.CAJ_ID = v.Caja;
    SET IDENTITY_INSERT Ventas OFF;

    INSERT INTO DetallesVenta (VEN_ID, LIB_ID, DV_Cantidad, DV_PrecioUnitario, DV_CostoUnitario)
    SELECT d.n, d.Lib, d.Cant, l.LIB_PrecioVenta, l.LIB_PrecioCosto
    FROM @vd d JOIN Libros l ON l.LIB_ID = d.Lib;

    INSERT INTO PagosVenta (VEN_ID, MP_ID, PAG_Monto, PAG_Recibido, PAG_Vuelto, PAG_Fecha)
    SELECT v.n, m.MP_ID, c.Total, c.Recibido, c.Vuelto, v.Fecha
    FROM @v v JOIN @calc c ON c.n = v.n JOIN @mp m ON m.k = v.Mp;

    /* ---------------------------------------------------------------------------------------
       10. Arqueo de las cajas cerradas (cuadradas: efectivo contado = efectivo esperado)
           Igual que CajaService: solo ventas no anuladas; esperado = inicial + ventas en efectivo.
       --------------------------------------------------------------------------------------- */
    INSERT INTO CajaTotalesMedio (CAJ_ID, MP_ID, CTM_MedioPago, CTM_EsEfectivo, CTM_Cantidad, CTM_Total)
    SELECT v.CAJ_ID, p.MP_ID, LEFT(mp.MP_Nombre, 60), ISNULL(m.EsEfectivo, 0), COUNT(DISTINCT v.VEN_ID), SUM(p.PAG_Monto)
    FROM Ventas v
    JOIN PagosVenta p ON p.VEN_ID = v.VEN_ID
    JOIN MetodosPago mp ON mp.MP_ID = p.MP_ID
    LEFT JOIN @mp m ON m.MP_ID = p.MP_ID
    JOIN Cajas c ON c.CAJ_ID = v.CAJ_ID AND c.CAJ_Estado = 1
    WHERE v.VEN_Anulada = 0
    GROUP BY v.CAJ_ID, p.MP_ID, mp.MP_Nombre, m.EsEfectivo;

    UPDATE c SET
        CAJ_CantidadVentas   = t.Cantidad,
        CAJ_TotalVentas      = t.Total,
        CAJ_EfectivoEsperado = c.CAJ_MontoInicial + t.Efectivo,
        CAJ_EfectivoContado  = c.CAJ_MontoInicial + t.Efectivo,
        CAJ_Diferencia       = 0
    FROM Cajas c
    JOIN (SELECT CAJ_ID, SUM(CTM_Cantidad) AS Cantidad, SUM(CTM_Total) AS Total,
                 SUM(CASE WHEN CTM_EsEfectivo = 1 THEN CTM_Total ELSE 0 END) AS Efectivo
          FROM CajaTotalesMedio GROUP BY CAJ_ID) t ON t.CAJ_ID = c.CAJ_ID
    WHERE c.CAJ_Estado = 1;

    /* ---------------------------------------------------------------------------------------
       11. Órdenes de reposición
           OC-000001 Recibida (hace 2 semanas) · OC-000002 Solicitada (ayer, libros críticos)
           OC-000003 Pendiente (borrador). Cantidad sugerida = stock óptimo − stock actual.
       --------------------------------------------------------------------------------------- */
    SET IDENTITY_INSERT OrdenesReposicion ON;
    INSERT INTO OrdenesReposicion (OR_ID, OR_PROV_ID, OR_Fecha, OR_Estado, OR_Usuario, OR_Observaciones,
                                   OR_FechaSolicitud, OR_FechaRecepcion, OR_UsuarioRecepcion) VALUES
        (1, 1, DATEADD(MINUTE, 600, DATEADD(DAY, -14, @hoy)), N'RECIBIDA', @nA, N'Reposición quincenal de clásicos.',
               DATEADD(MINUTE, 630, DATEADD(DAY, -14, @hoy)), DATEADD(MINUTE, 660, DATEADD(DAY, -12, @hoy)), @nA),
        (2, 2, DATEADD(MINUTE, 600, @b2), N'SOLICITADA', @nB, N'Urgente: títulos en stock crítico.',
               DATEADD(MINUTE, 615, @b2), NULL, NULL),
        (3, 4, DATEADD(MINUTE, 630, @b2), N'PENDIENTE', @nB, N'Confirmar disponibilidad de la nueva edición antes de emitir.',
               NULL, NULL, NULL);
    SET IDENTITY_INSERT OrdenesReposicion OFF;

    INSERT INTO DetallesOrdenReposicion (OR_ID, LIB_ID, DOR_CantidadPedida, DOR_CostoUnitario, DOR_CantidadRecibida, DOR_CostoRecibido)
    SELECT o.OrdenId, o.Lib,
           ISNULL(o.Cant, l.LIB_StockOptimo - l.LIB_Stock),
           pl.PL_PrecioCompra,
           CASE WHEN o.OrdenId = 1 THEN o.Cant END,
           CASE WHEN o.OrdenId = 1 THEN pl.PL_PrecioCompra END
    FROM (VALUES (1, 1, 10), (1, 3, 8),                 -- recibida: cantidades fijas
                 (2, 6, NULL), (2, 12, NULL),           -- solicitada: hasta el óptimo
                 (3, 5, NULL), (3, 11, 6)) o(OrdenId, Lib, Cant)
    JOIN Libros l ON l.LIB_ID = o.Lib
    JOIN OrdenesReposicion r ON r.OR_ID = o.OrdenId
    JOIN ProveedoresLibros pl ON pl.PROV_ID = r.OR_PROV_ID AND pl.LIB_ID = o.Lib;

    /* ---------------------------------------------------------------------------------------
       12. Historial de stock (kardex)
           Cada libro arranca con un ALTA_INICIAL hace 20 días tal que, aplicando en orden ventas,
           anulaciones y recepciones, el stock termina exactamente en LIB_Stock.
       --------------------------------------------------------------------------------------- */
    DECLARE @ev TABLE (Seq int IDENTITY(1,1), LIB_ID int, Fecha datetime2, Delta int, Tipo nvarchar(30),
                       Ref nvarchar(30), Usuario nvarchar(100), Motivo nvarchar(100), Obs nvarchar(250));

    INSERT INTO @ev (LIB_ID, Fecha, Delta, Tipo, Ref, Usuario, Motivo, Obs)
    SELECT d.LIB_ID, v.VEN_Fecha, -d.DV_Cantidad, N'VENTA', N'TK-' + RIGHT('000000' + CAST(v.VEN_ID AS varchar(10)), 6),
           v.VEN_Usuario, N'Venta', NULL
    FROM DetallesVenta d JOIN Ventas v ON v.VEN_ID = d.VEN_ID
    UNION ALL
    SELECT d.LIB_ID, v.VEN_FechaAnulacion, d.DV_Cantidad, N'ANULACION_VENTA', N'TK-' + RIGHT('000000' + CAST(v.VEN_ID AS varchar(10)), 6),
           v.VEN_UsuarioAnulacion, N'Anulación de venta', v.VEN_MotivoAnulacion
    FROM DetallesVenta d JOIN Ventas v ON v.VEN_ID = d.VEN_ID
    WHERE v.VEN_Anulada = 1
    UNION ALL
    SELECT d.LIB_ID, o.OR_FechaRecepcion, d.DOR_CantidadRecibida, N'RECEPCION_ORDEN', N'OC-' + RIGHT('000000' + CAST(o.OR_ID AS varchar(10)), 6),
           o.OR_UsuarioRecepcion, N'Recepción de mercadería', NULL
    FROM DetallesOrdenReposicion d JOIN OrdenesReposicion o ON o.OR_ID = d.OR_ID
    WHERE o.OR_Estado = N'RECIBIDA' AND d.DOR_CantidadRecibida > 0;

    DECLARE @ini TABLE (LIB_ID int PRIMARY KEY, Inicial int);
    INSERT INTO @ini
    SELECT l.LIB_ID, l.LIB_Stock - ISNULL(SUM(e.Delta), 0)
    FROM Libros l LEFT JOIN @ev e ON e.LIB_ID = l.LIB_ID
    GROUP BY l.LIB_ID, l.LIB_Stock;

    IF EXISTS (SELECT 1 FROM @ini WHERE Inicial < 0)
        THROW 50002, N'Algún libro quedaría con stock inicial negativo: revisar ventas y stock final.', 1;

    INSERT INTO MovimientosStock (LIB_ID, MOV_Fecha, MOV_Tipo, MOV_Cantidad, MOV_StockAnterior, MOV_StockResultante,
                                  MOV_Motivo, MOV_Observacion, MOV_Usuario, MOV_Referencia)
    SELECT LIB_ID, DATEADD(MINUTE, 570, DATEADD(DAY, -20, @hoy)), N'ALTA_INICIAL', Inicial, 0, Inicial,
           N'Alta del libro', NULL, @nA, NULL
    FROM @ini;

    INSERT INTO MovimientosStock (LIB_ID, MOV_Fecha, MOV_Tipo, MOV_Cantidad, MOV_StockAnterior, MOV_StockResultante,
                                  MOV_Motivo, MOV_Observacion, MOV_Usuario, MOV_Referencia)
    SELECT e.LIB_ID, e.Fecha, e.Tipo, e.Delta,
           i.Inicial + SUM(e.Delta) OVER (PARTITION BY e.LIB_ID ORDER BY e.Fecha, e.Seq ROWS UNBOUNDED PRECEDING) - e.Delta,
           i.Inicial + SUM(e.Delta) OVER (PARTITION BY e.LIB_ID ORDER BY e.Fecha, e.Seq ROWS UNBOUNDED PRECEDING),
           e.Motivo, e.Obs, e.Usuario, e.Ref
    FROM @ev e JOIN @ini i ON i.LIB_ID = e.LIB_ID;

    COMMIT TRANSACTION;

    /* ---------------------------------------------------------------------------------------
       Resumen
       --------------------------------------------------------------------------------------- */
    SELECT N'Géneros' AS Tabla, COUNT(*) AS Filas FROM Generos
    UNION ALL SELECT N'Libros', COUNT(*) FROM Libros
    UNION ALL SELECT N'Clientes', COUNT(*) FROM Clientes
    UNION ALL SELECT N'Proveedores', COUNT(*) FROM Proveedores
    UNION ALL SELECT N'Cajas', COUNT(*) FROM Cajas
    UNION ALL SELECT N'Ventas', COUNT(*) FROM Ventas
    UNION ALL SELECT N'Detalles de venta', COUNT(*) FROM DetallesVenta
    UNION ALL SELECT N'Órdenes de reposición', COUNT(*) FROM OrdenesReposicion
    UNION ALL SELECT N'Movimientos de stock', COUNT(*) FROM MovimientosStock
    UNION ALL SELECT N'Usuarios (sin cambios)', COUNT(*) FROM Usuarios;

    PRINT N'Datos de prueba cargados correctamente.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @msg nvarchar(4000) = N'No se cargaron los datos (se deshizo todo). Error ' + CAST(ERROR_NUMBER() AS nvarchar(10))
        + N' en la línea ' + CAST(ERROR_LINE() AS nvarchar(10)) + N': ' + ERROR_MESSAGE();
    RAISERROR (@msg, 16, 1);
END CATCH;
