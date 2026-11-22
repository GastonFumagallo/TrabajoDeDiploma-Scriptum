IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Estados_Grupos] (
        [EST_GRU_ID] int NOT NULL IDENTITY,
        [EST_GRU_Nombre] nvarchar(60) NOT NULL,
        CONSTRAINT [PK_Estados_Grupos] PRIMARY KEY ([EST_GRU_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Estados_Usuarios] (
        [EST_USU_ID] int NOT NULL IDENTITY,
        [EST_USU_Nombre] nvarchar(60) NOT NULL,
        CONSTRAINT [PK_Estados_Usuarios] PRIMARY KEY ([EST_USU_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Generos] (
        [GEN_ID] int NOT NULL IDENTITY,
        [GEN_Nombre] nvarchar(max) NOT NULL,
        [GEN_Descripcion] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Generos] PRIMARY KEY ([GEN_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [MetodosPago] (
        [MP_ID] int NOT NULL IDENTITY,
        [MP_Nombre] nvarchar(max) NOT NULL,
        [MP_Estado] bit NOT NULL,
        CONSTRAINT [PK_MetodosPago] PRIMARY KEY ([MP_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Modulos] (
        [MOD_ID] int NOT NULL IDENTITY,
        [MOD_Nombre] nvarchar(60) NOT NULL,
        CONSTRAINT [PK_Modulos] PRIMARY KEY ([MOD_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Personas] (
        [PER_ID] int NOT NULL IDENTITY,
        [PER_Nombre] nvarchar(60) NOT NULL,
        [PER_Mail] nvarchar(max) NOT NULL,
        [PER_Telefono] nvarchar(max) NOT NULL,
        [PER_DNI] int NOT NULL,
        CONSTRAINT [PK_Personas] PRIMARY KEY ([PER_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Grupos] (
        [GRU_ID] int NOT NULL IDENTITY,
        [GRU_Nombre] nvarchar(60) NOT NULL,
        [GRU_Descripcion] nvarchar(60) NOT NULL,
        [EST_GRU_ID] int NOT NULL,
        CONSTRAINT [PK_Grupos] PRIMARY KEY ([GRU_ID]),
        CONSTRAINT [FK_Grupos_Estados_Grupos_EST_GRU_ID] FOREIGN KEY ([EST_GRU_ID]) REFERENCES [Estados_Grupos] ([EST_GRU_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Libros] (
        [LIB_ID] int NOT NULL IDENTITY,
        [LIB_Titulo] nvarchar(max) NOT NULL,
        [LIB_Autor] nvarchar(max) NOT NULL,
        [LIB_Descripcion] nvarchar(max) NOT NULL,
        [LIB_Editorial] nvarchar(max) NOT NULL,
        [LIB_AñoPublicacion] int NOT NULL,
        [GEN_ID] int NOT NULL,
        [LIB_GeneroGEN_ID] int NOT NULL,
        CONSTRAINT [PK_Libros] PRIMARY KEY ([LIB_ID]),
        CONSTRAINT [FK_Libros_Generos_LIB_GeneroGEN_ID] FOREIGN KEY ([LIB_GeneroGEN_ID]) REFERENCES [Generos] ([GEN_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Formularios] (
        [FORM_ID] int NOT NULL IDENTITY,
        [FORM_Nombre] nvarchar(60) NOT NULL,
        [MOD_ID] int NULL,
        CONSTRAINT [PK_Formularios] PRIMARY KEY ([FORM_ID]),
        CONSTRAINT [FK_Formularios_Modulos_MOD_ID] FOREIGN KEY ([MOD_ID]) REFERENCES [Modulos] ([MOD_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Clientes] (
        [CLI_ID] int NOT NULL IDENTITY,
        [PER_ID] int NOT NULL,
        [CLI_PersonaPER_ID] int NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([CLI_ID]),
        CONSTRAINT [FK_Clientes_Personas_CLI_PersonaPER_ID] FOREIGN KEY ([CLI_PersonaPER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Proveedores] (
        [PROV_ID] int NOT NULL IDENTITY,
        [PER_ID] int NOT NULL,
        [PER_ProveedorPER_ID] int NOT NULL,
        [PROV_Empresa] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Proveedores] PRIMARY KEY ([PROV_ID]),
        CONSTRAINT [FK_Proveedores_Personas_PER_ProveedorPER_ID] FOREIGN KEY ([PER_ProveedorPER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [USU_ID] int NOT NULL IDENTITY,
        [USU_Nombre] nvarchar(60) NOT NULL,
        [USU_Clave] nvarchar(64) NOT NULL,
        [USU_Mail] nvarchar(60) NOT NULL,
        [EST_USU_ID] int NULL,
        [PER_ID] int NOT NULL,
        [USU_PersonaPER_ID] int NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([USU_ID]),
        CONSTRAINT [FK_Usuarios_Estados_Usuarios_EST_USU_ID] FOREIGN KEY ([EST_USU_ID]) REFERENCES [Estados_Usuarios] ([EST_USU_ID]),
        CONSTRAINT [FK_Usuarios_Personas_USU_PersonaPER_ID] FOREIGN KEY ([USU_PersonaPER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Acciones] (
        [ACC_ID] int NOT NULL IDENTITY,
        [ACC_Nombre] nvarchar(60) NOT NULL,
        [FORM_ID] int NULL,
        CONSTRAINT [PK_Acciones] PRIMARY KEY ([ACC_ID]),
        CONSTRAINT [FK_Acciones_Formularios_FORM_ID] FOREIGN KEY ([FORM_ID]) REFERENCES [Formularios] ([FORM_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [Ventas] (
        [VEN_ID] int NOT NULL IDENTITY,
        [VEN_Fecha] datetime2 NOT NULL,
        [CLI_ID] int NOT NULL,
        [VEN_ClienteCLI_ID] int NOT NULL,
        [VEN_Total] decimal(18,2) NOT NULL,
        [MP_ID] int NOT NULL,
        [VEN_MetodoPagoMP_ID] int NOT NULL,
        CONSTRAINT [PK_Ventas] PRIMARY KEY ([VEN_ID]),
        CONSTRAINT [FK_Ventas_Clientes_VEN_ClienteCLI_ID] FOREIGN KEY ([VEN_ClienteCLI_ID]) REFERENCES [Clientes] ([CLI_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Ventas_MetodosPago_VEN_MetodoPagoMP_ID] FOREIGN KEY ([VEN_MetodoPagoMP_ID]) REFERENCES [MetodosPago] ([MP_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [ProveedorLibro] (
        [PROV_ID] int NOT NULL,
        [LIB_ID] int NOT NULL,
        [Precio] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_ProveedorLibro] PRIMARY KEY ([PROV_ID], [LIB_ID]),
        CONSTRAINT [FK_ProveedorLibro_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProveedorLibro_Proveedores_PROV_ID] FOREIGN KEY ([PROV_ID]) REFERENCES [Proveedores] ([PROV_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [AuditoriaSesiones] (
        [AS_ID] int NOT NULL IDENTITY,
        [AS_USU_ID] int NOT NULL,
        [AS_UsuarioUSU_ID] int NOT NULL,
        [AS_FechaHoraLogin] datetime2 NOT NULL,
        [AS_FechaHoraLogout] datetime2 NULL,
        [AS_TipoLogout] nvarchar(max) NULL,
        [AS_SesionActiva] bit NOT NULL,
        [AS_TiempoSesion] int NULL,
        CONSTRAINT [PK_AuditoriaSesiones] PRIMARY KEY ([AS_ID]),
        CONSTRAINT [FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID] FOREIGN KEY ([AS_UsuarioUSU_ID]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [GrupoUsuario] (
        [GruposGRU_ID] int NOT NULL,
        [UsuariosUSU_ID] int NOT NULL,
        CONSTRAINT [PK_GrupoUsuario] PRIMARY KEY ([GruposGRU_ID], [UsuariosUSU_ID]),
        CONSTRAINT [FK_GrupoUsuario_Grupos_GruposGRU_ID] FOREIGN KEY ([GruposGRU_ID]) REFERENCES [Grupos] ([GRU_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_GrupoUsuario_Usuarios_UsuariosUSU_ID] FOREIGN KEY ([UsuariosUSU_ID]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [AccionGrupo] (
        [AccionesACC_ID] int NOT NULL,
        [GruposGRU_ID] int NOT NULL,
        CONSTRAINT [PK_AccionGrupo] PRIMARY KEY ([AccionesACC_ID], [GruposGRU_ID]),
        CONSTRAINT [FK_AccionGrupo_Acciones_AccionesACC_ID] FOREIGN KEY ([AccionesACC_ID]) REFERENCES [Acciones] ([ACC_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_AccionGrupo_Grupos_GruposGRU_ID] FOREIGN KEY ([GruposGRU_ID]) REFERENCES [Grupos] ([GRU_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [AccionUsuario] (
        [AccionesACC_ID] int NOT NULL,
        [UsuariosUSU_ID] int NOT NULL,
        CONSTRAINT [PK_AccionUsuario] PRIMARY KEY ([AccionesACC_ID], [UsuariosUSU_ID]),
        CONSTRAINT [FK_AccionUsuario_Acciones_AccionesACC_ID] FOREIGN KEY ([AccionesACC_ID]) REFERENCES [Acciones] ([ACC_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_AccionUsuario_Usuarios_UsuariosUSU_ID] FOREIGN KEY ([UsuariosUSU_ID]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE TABLE [DetallesVenta] (
        [DV_ID] int NOT NULL IDENTITY,
        [VEN_ID] int NOT NULL,
        [DV_VentaVEN_ID] int NOT NULL,
        [LIB_ID] int NOT NULL,
        [DV_LibroLIB_ID] int NOT NULL,
        [DV_Cantidad] int NOT NULL,
        [DV_PrecioUnitario] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_DetallesVenta] PRIMARY KEY ([DV_ID]),
        CONSTRAINT [FK_DetallesVenta_Libros_DV_LibroLIB_ID] FOREIGN KEY ([DV_LibroLIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_DetallesVenta_Ventas_DV_VentaVEN_ID] FOREIGN KEY ([DV_VentaVEN_ID]) REFERENCES [Ventas] ([VEN_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Acciones_FORM_ID] ON [Acciones] ([FORM_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_AccionGrupo_GruposGRU_ID] ON [AccionGrupo] ([GruposGRU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_AccionUsuario_UsuariosUSU_ID] ON [AccionUsuario] ([UsuariosUSU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_AuditoriaSesiones_AS_UsuarioUSU_ID] ON [AuditoriaSesiones] ([AS_UsuarioUSU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Clientes_CLI_PersonaPER_ID] ON [Clientes] ([CLI_PersonaPER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_DV_LibroLIB_ID] ON [DetallesVenta] ([DV_LibroLIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_DV_VentaVEN_ID] ON [DetallesVenta] ([DV_VentaVEN_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Formularios_MOD_ID] ON [Formularios] ([MOD_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Grupos_EST_GRU_ID] ON [Grupos] ([EST_GRU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_GrupoUsuario_UsuariosUSU_ID] ON [GrupoUsuario] ([UsuariosUSU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Libros_LIB_GeneroGEN_ID] ON [Libros] ([LIB_GeneroGEN_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Proveedores_PER_ProveedorPER_ID] ON [Proveedores] ([PER_ProveedorPER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_ProveedorLibro_LIB_ID] ON [ProveedorLibro] ([LIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Usuarios_EST_USU_ID] ON [Usuarios] ([EST_USU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Usuarios_USU_PersonaPER_ID] ON [Usuarios] ([USU_PersonaPER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Ventas_VEN_ClienteCLI_ID] ON [Ventas] ([VEN_ClienteCLI_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    CREATE INDEX [IX_Ventas_VEN_MetodoPagoMP_ID] ON [Ventas] ([VEN_MetodoPagoMP_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260501192721_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260501192721_Inicial', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511002421_AgregarStockLibro'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_Stock] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511002421_AgregarStockLibro'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260511002421_AgregarStockLibro', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedorLibro] DROP CONSTRAINT [FK_ProveedorLibro_Libros_LIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedorLibro] DROP CONSTRAINT [FK_ProveedorLibro_Proveedores_PROV_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedorLibro] DROP CONSTRAINT [PK_ProveedorLibro];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    EXEC sp_rename N'[ProveedorLibro]', N'ProveedoresLibros', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    EXEC sp_rename N'[ProveedoresLibros].[IX_ProveedorLibro_LIB_ID]', N'IX_ProveedoresLibros_LIB_ID', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedoresLibros] ADD CONSTRAINT [PK_ProveedoresLibros] PRIMARY KEY ([PROV_ID], [LIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedoresLibros] ADD CONSTRAINT [FK_ProveedoresLibros_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    ALTER TABLE [ProveedoresLibros] ADD CONSTRAINT [FK_ProveedoresLibros_Proveedores_PROV_ID] FOREIGN KEY ([PROV_ID]) REFERENCES [Proveedores] ([PROV_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511021058_AgregarProveedorLibro'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260511021058_AgregarProveedorLibro', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260522210632_PrecioLibro'
)
BEGIN
    EXEC sp_rename N'[ProveedoresLibros].[Precio]', N'PL_PrecioCompra', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260522210632_PrecioLibro'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_PrecioVenta] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260522210632_PrecioLibro'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260522210632_PrecioLibro', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607161253_OrdenReposicion'
)
BEGIN
    CREATE TABLE [OrdenesReposicion] (
        [OR_ID] int NOT NULL IDENTITY,
        [OR_LIB_ID] int NOT NULL,
        [OR_PROV_ID] int NOT NULL,
        [OR_Cantidad] int NOT NULL,
        [OR_Fecha] datetime2 NOT NULL,
        [OR_Estado] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_OrdenesReposicion] PRIMARY KEY ([OR_ID]),
        CONSTRAINT [FK_OrdenesReposicion_Libros_OR_LIB_ID] FOREIGN KEY ([OR_LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesReposicion_Proveedores_OR_PROV_ID] FOREIGN KEY ([OR_PROV_ID]) REFERENCES [Proveedores] ([PROV_ID]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607161253_OrdenReposicion'
)
BEGIN
    CREATE INDEX [IX_OrdenesReposicion_OR_LIB_ID] ON [OrdenesReposicion] ([OR_LIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607161253_OrdenReposicion'
)
BEGIN
    CREATE INDEX [IX_OrdenesReposicion_OR_PROV_ID] ON [OrdenesReposicion] ([OR_PROV_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607161253_OrdenReposicion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607161253_OrdenReposicion', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [DetallesVenta] DROP CONSTRAINT [FK_DetallesVenta_Libros_DV_LibroLIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [DetallesVenta] DROP CONSTRAINT [FK_DetallesVenta_Ventas_DV_VentaVEN_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Libros] DROP CONSTRAINT [FK_Libros_Generos_LIB_GeneroGEN_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_Clientes_VEN_ClienteCLI_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_MetodosPago_VEN_MetodoPagoMP_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DROP INDEX [IX_Ventas_VEN_ClienteCLI_ID] ON [Ventas];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DROP INDEX [IX_Ventas_VEN_MetodoPagoMP_ID] ON [Ventas];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DROP INDEX [IX_Libros_LIB_GeneroGEN_ID] ON [Libros];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DROP INDEX [IX_DetallesVenta_DV_LibroLIB_ID] ON [DetallesVenta];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DROP INDEX [IX_DetallesVenta_DV_VentaVEN_ID] ON [DetallesVenta];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    UPDATE Ventas SET CLI_ID = VEN_ClienteCLI_ID, MP_ID = VEN_MetodoPagoMP_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    UPDATE Libros SET GEN_ID = LIB_GeneroGEN_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    UPDATE DetallesVenta SET LIB_ID = DV_LibroLIB_ID, VEN_ID = DV_VentaVEN_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ventas]') AND [c].[name] = N'VEN_ClienteCLI_ID');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Ventas] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Ventas] DROP COLUMN [VEN_ClienteCLI_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ventas]') AND [c].[name] = N'VEN_MetodoPagoMP_ID');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Ventas] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Ventas] DROP COLUMN [VEN_MetodoPagoMP_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Libros]') AND [c].[name] = N'LIB_GeneroGEN_ID');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Libros] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Libros] DROP COLUMN [LIB_GeneroGEN_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DetallesVenta]') AND [c].[name] = N'DV_LibroLIB_ID');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [DetallesVenta] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [DetallesVenta] DROP COLUMN [DV_LibroLIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DetallesVenta]') AND [c].[name] = N'DV_VentaVEN_ID');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [DetallesVenta] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [DetallesVenta] DROP COLUMN [DV_VentaVEN_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    CREATE INDEX [IX_Ventas_CLI_ID] ON [Ventas] ([CLI_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    CREATE INDEX [IX_Ventas_MP_ID] ON [Ventas] ([MP_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    CREATE INDEX [IX_Libros_GEN_ID] ON [Libros] ([GEN_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_LIB_ID] ON [DetallesVenta] ([LIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_VEN_ID] ON [DetallesVenta] ([VEN_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [DetallesVenta] ADD CONSTRAINT [FK_DetallesVenta_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [DetallesVenta] ADD CONSTRAINT [FK_DetallesVenta_Ventas_VEN_ID] FOREIGN KEY ([VEN_ID]) REFERENCES [Ventas] ([VEN_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Libros] ADD CONSTRAINT [FK_Libros_Generos_GEN_ID] FOREIGN KEY ([GEN_ID]) REFERENCES [Generos] ([GEN_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Ventas] ADD CONSTRAINT [FK_Ventas_Clientes_CLI_ID] FOREIGN KEY ([CLI_ID]) REFERENCES [Clientes] ([CLI_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    ALTER TABLE [Ventas] ADD CONSTRAINT [FK_Ventas_MetodosPago_MP_ID] FOREIGN KEY ([MP_ID]) REFERENCES [MetodosPago] ([MP_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002013822_CorregirClavesForaneas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002013822_CorregirClavesForaneas', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [DetallesVenta] DROP CONSTRAINT [FK_DetallesVenta_Libros_LIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Libros] DROP CONSTRAINT [FK_Libros_Generos_GEN_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_Clientes_CLI_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_MetodosPago_MP_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [DetallesVenta] ADD CONSTRAINT [FK_DetallesVenta_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Libros] ADD CONSTRAINT [FK_Libros_Generos_GEN_ID] FOREIGN KEY ([GEN_ID]) REFERENCES [Generos] ([GEN_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD CONSTRAINT [FK_Ventas_Clientes_CLI_ID] FOREIGN KEY ([CLI_ID]) REFERENCES [Clientes] ([CLI_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD CONSTRAINT [FK_Ventas_MetodosPago_MP_ID] FOREIGN KEY ([MP_ID]) REFERENCES [MetodosPago] ([MP_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002015719_RestringirBorradoEnVentas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002015719_RestringirBorradoEnVentas', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002020046_AnulacionVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_Anulada] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002020046_AnulacionVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_FechaAnulacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002020046_AnulacionVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_MotivoAnulacion] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002020046_AnulacionVentas'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_UsuarioAnulacion] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002020046_AnulacionVentas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002020046_AnulacionVentas', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_AjusteMedioPago] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_Descuento] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_IVA] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_PorcentajeDescuento] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_Subtotal] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Ventas] ADD [VEN_Usuario] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_ISBN] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    CREATE TABLE [PagosVenta] (
        [PAG_ID] int NOT NULL IDENTITY,
        [VEN_ID] int NOT NULL,
        [MP_ID] int NOT NULL,
        [PAG_Monto] decimal(18,2) NOT NULL,
        [PAG_Recibido] decimal(18,2) NOT NULL,
        [PAG_Vuelto] decimal(18,2) NOT NULL,
        [PAG_Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_PagosVenta] PRIMARY KEY ([PAG_ID]),
        CONSTRAINT [FK_PagosVenta_MetodosPago_MP_ID] FOREIGN KEY ([MP_ID]) REFERENCES [MetodosPago] ([MP_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PagosVenta_Ventas_VEN_ID] FOREIGN KEY ([VEN_ID]) REFERENCES [Ventas] ([VEN_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    CREATE INDEX [IX_Libros_LIB_ISBN] ON [Libros] ([LIB_ISBN]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    CREATE INDEX [IX_PagosVenta_MP_ID] ON [PagosVenta] ([MP_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    CREATE INDEX [IX_PagosVenta_VEN_ID] ON [PagosVenta] ([VEN_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN

    UPDATE v SET
        VEN_Subtotal = ISNULL(d.Subtotal, v.VEN_Total),
        VEN_AjusteMedioPago = v.VEN_Total - ISNULL(d.Subtotal, v.VEN_Total)
    FROM Ventas v
    OUTER APPLY (SELECT SUM(dv.DV_Cantidad * dv.DV_PrecioUnitario) AS Subtotal
                 FROM DetallesVenta dv WHERE dv.VEN_ID = v.VEN_ID) d;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN

    INSERT INTO PagosVenta (VEN_ID, MP_ID, PAG_Monto, PAG_Recibido, PAG_Vuelto, PAG_Fecha)
    SELECT v.VEN_ID, v.MP_ID, v.VEN_Total, v.VEN_Total, 0, v.VEN_Fecha
    FROM Ventas v
    WHERE NOT EXISTS (SELECT 1 FROM PagosVenta p WHERE p.VEN_ID = v.VEN_ID);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002162413_PuntoDeVenta'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002162413_PuntoDeVenta', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] DROP CONSTRAINT [FK_OrdenesReposicion_Libros_OR_LIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    DROP INDEX [IX_OrdenesReposicion_OR_LIB_ID] ON [OrdenesReposicion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrdenesReposicion]') AND [c].[name] = N'OR_Estado');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [OrdenesReposicion] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [OrdenesReposicion] ALTER COLUMN [OR_Estado] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_FechaCancelacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_FechaRecepcion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_FechaSolicitud] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_MotivoCancelacion] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_Observaciones] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_Usuario] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_UsuarioCancelacion] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [OrdenesReposicion] ADD [OR_UsuarioRecepcion] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_PrecioCosto] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_PuntoReposicion] int NOT NULL DEFAULT 5;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_StockMinimo] int NOT NULL DEFAULT 2;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_StockOptimo] int NOT NULL DEFAULT 10;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE TABLE [DetallesOrdenReposicion] (
        [DOR_ID] int NOT NULL IDENTITY,
        [OR_ID] int NOT NULL,
        [LIB_ID] int NOT NULL,
        [DOR_CantidadPedida] int NOT NULL,
        [DOR_CostoUnitario] decimal(18,2) NOT NULL,
        [DOR_CantidadRecibida] int NULL,
        [DOR_CostoRecibido] decimal(18,2) NULL,
        CONSTRAINT [PK_DetallesOrdenReposicion] PRIMARY KEY ([DOR_ID]),
        CONSTRAINT [FK_DetallesOrdenReposicion_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetallesOrdenReposicion_OrdenesReposicion_OR_ID] FOREIGN KEY ([OR_ID]) REFERENCES [OrdenesReposicion] ([OR_ID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE TABLE [MovimientosStock] (
        [MOV_ID] int NOT NULL IDENTITY,
        [LIB_ID] int NOT NULL,
        [MOV_Fecha] datetime2 NOT NULL,
        [MOV_Tipo] nvarchar(30) NOT NULL,
        [MOV_Cantidad] int NOT NULL,
        [MOV_StockAnterior] int NOT NULL,
        [MOV_StockResultante] int NOT NULL,
        [MOV_Motivo] nvarchar(100) NULL,
        [MOV_Observacion] nvarchar(250) NULL,
        [MOV_Usuario] nvarchar(100) NULL,
        [MOV_Referencia] nvarchar(30) NULL,
        CONSTRAINT [PK_MovimientosStock] PRIMARY KEY ([MOV_ID]),
        CONSTRAINT [FK_MovimientosStock_Libros_LIB_ID] FOREIGN KEY ([LIB_ID]) REFERENCES [Libros] ([LIB_ID]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE INDEX [IX_OrdenesReposicion_OR_Estado] ON [OrdenesReposicion] ([OR_Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE INDEX [IX_DetallesOrdenReposicion_LIB_ID] ON [DetallesOrdenReposicion] ([LIB_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE INDEX [IX_DetallesOrdenReposicion_OR_ID] ON [DetallesOrdenReposicion] ([OR_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    CREATE INDEX [IX_MovimientosStock_LIB_ID_MOV_Fecha] ON [MovimientosStock] ([LIB_ID], [MOV_Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN

    INSERT INTO DetallesOrdenReposicion (OR_ID, LIB_ID, DOR_CantidadPedida, DOR_CostoUnitario, DOR_CantidadRecibida, DOR_CostoRecibido)
    SELECT o.OR_ID, o.OR_LIB_ID, o.OR_Cantidad,
           ISNULL(pl.PL_PrecioCompra, 0),
           CASE WHEN o.OR_Estado = 'RECIBIDA' THEN o.OR_Cantidad END,
           CASE WHEN o.OR_Estado = 'RECIBIDA' THEN ISNULL(pl.PL_PrecioCompra, 0) END
    FROM OrdenesReposicion o
    LEFT JOIN ProveedoresLibros pl ON pl.PROV_ID = o.OR_PROV_ID AND pl.LIB_ID = o.OR_LIB_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    UPDATE OrdenesReposicion SET OR_Estado = 'SOLICITADA', OR_FechaSolicitud = OR_Fecha WHERE OR_Estado = 'ENVIADA';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    UPDATE OrdenesReposicion SET OR_FechaRecepcion = OR_Fecha WHERE OR_Estado = 'RECIBIDA';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN

    UPDATE l SET LIB_PrecioCosto = p.Minimo
    FROM Libros l
    JOIN (SELECT LIB_ID, MIN(PL_PrecioCompra) AS Minimo FROM ProveedoresLibros GROUP BY LIB_ID) p ON p.LIB_ID = l.LIB_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN

    INSERT INTO MovimientosStock (LIB_ID, MOV_Fecha, MOV_Tipo, MOV_Cantidad, MOV_StockAnterior, MOV_StockResultante, MOV_Motivo, MOV_Usuario)
    SELECT LIB_ID, SYSDATETIME(), 'ALTA_INICIAL', LIB_Stock, 0, LIB_Stock, 'Saldo inicial al habilitar el historial', 'sistema'
    FROM Libros WHERE LIB_Stock > 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrdenesReposicion]') AND [c].[name] = N'OR_Cantidad');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [OrdenesReposicion] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [OrdenesReposicion] DROP COLUMN [OR_Cantidad];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrdenesReposicion]') AND [c].[name] = N'OR_LIB_ID');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [OrdenesReposicion] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [OrdenesReposicion] DROP COLUMN [OR_LIB_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003042349_InventarioYReposicion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261003042349_InventarioYReposicion', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN
    DROP INDEX [IX_Libros_LIB_ISBN] ON [Libros];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_Activo] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN
    ALTER TABLE [Libros] ADD [LIB_Version] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN

    WITH Repetidos AS (
        SELECT LIB_ID, ROW_NUMBER() OVER (PARTITION BY LIB_ISBN ORDER BY LIB_ID) AS Orden
        FROM Libros WHERE LIB_ISBN IS NOT NULL
    )
    UPDATE Libros SET LIB_ISBN = NULL
    WHERE LIB_ID IN (SELECT LIB_ID FROM Repetidos WHERE Orden > 1);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Libros_LIB_ISBN] ON [Libros] ([LIB_ISBN]) WHERE [LIB_ISBN] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003044021_AbmLibros'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261003044021_AbmLibros', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] DROP CONSTRAINT [FK_Clientes_Personas_CLI_PersonaPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] DROP CONSTRAINT [FK_Proveedores_Personas_PER_ProveedorPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    DROP INDEX [IX_Proveedores_PER_ProveedorPER_ID] ON [Proveedores];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    DROP INDEX [IX_Clientes_CLI_PersonaPER_ID] ON [Clientes];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    UPDATE Proveedores SET PER_ID = PER_ProveedorPER_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    UPDATE Clientes SET PER_ID = CLI_PersonaPER_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Proveedores]') AND [c].[name] = N'PER_ProveedorPER_ID');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Proveedores] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [Proveedores] DROP COLUMN [PER_ProveedorPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clientes]') AND [c].[name] = N'CLI_PersonaPER_ID');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Clientes] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [Clientes] DROP COLUMN [CLI_PersonaPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_Version] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_Activo] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_CUIT] nvarchar(11) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_CondicionFiscal] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_Direccion] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD [PROV_Observaciones] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_Version] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_Activo] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_ConsumidorFinal] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_Direccion] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_Documento] nvarchar(11) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_LimiteCredito] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_Localidad] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CLI_TipoDocumento] nvarchar(4) NOT NULL DEFAULT N'DNI';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN

    UPDATE Clientes SET CLI_ConsumidorFinal = 1
    WHERE CLI_ID = (SELECT MIN(c.CLI_ID) FROM Clientes c
                    JOIN Personas p ON p.PER_ID = c.PER_ID
                    WHERE p.PER_DNI = 0 AND p.PER_Nombre = 'Consumidor Final');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN

    WITH Documentos AS (
        SELECT c.CLI_ID, CAST(p.PER_DNI AS nvarchar(11)) AS Documento,
               ROW_NUMBER() OVER (PARTITION BY p.PER_DNI ORDER BY c.CLI_ID) AS Orden
        FROM Clientes c JOIN Personas p ON p.PER_ID = c.PER_ID
        WHERE p.PER_DNI BETWEEN 1000000 AND 99999999 AND c.CLI_ConsumidorFinal = 0
    )
    UPDATE c SET CLI_Documento = d.Documento
    FROM Clientes c JOIN Documentos d ON d.CLI_ID = c.CLI_ID
    WHERE d.Orden = 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    CREATE INDEX [IX_Proveedores_PER_ID] ON [Proveedores] ([PER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Proveedores_PROV_CUIT] ON [Proveedores] ([PROV_CUIT]) WHERE [PROV_CUIT] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Clientes_CLI_Documento] ON [Clientes] ([CLI_Documento]) WHERE [CLI_Documento] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    CREATE INDEX [IX_Clientes_PER_ID] ON [Clientes] ([PER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Clientes] ADD CONSTRAINT [FK_Clientes_Personas_PER_ID] FOREIGN KEY ([PER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    ALTER TABLE [Proveedores] ADD CONSTRAINT [FK_Proveedores_Personas_PER_ID] FOREIGN KEY ([PER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003132112_AbmProveedoresClientes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261003132112_AbmProveedoresClientes', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003135614_CostoEnDetalleVenta'
)
BEGIN
    ALTER TABLE [DetallesVenta] ADD [DV_CostoUnitario] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003135614_CostoEnDetalleVenta'
)
BEGIN

    UPDATE dv SET DV_CostoUnitario = l.LIB_PrecioCosto
    FROM DetallesVenta dv JOIN Libros l ON l.LIB_ID = dv.LIB_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261003135614_CostoEnDetalleVenta'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261003135614_CostoEnDetalleVenta', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [AuditoriaSesiones] DROP CONSTRAINT [FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'USU_Clave');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Usuarios] ALTER COLUMN [USU_Clave] nvarchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_BloqueadoHasta] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_ClaveTemporal] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_ClaveTemporalVence] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_DebeCambiarClave] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_IntentosFallidos] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    CREATE TABLE [AuditoriaSeguridad] (
        [AUD_ID] int NOT NULL IDENTITY,
        [AUD_Fecha] datetime2 NOT NULL,
        [AUD_Usuario] nvarchar(60) NULL,
        [AUD_Evento] nvarchar(40) NOT NULL,
        [AUD_Detalle] nvarchar(400) NULL,
        [AUD_Equipo] nvarchar(100) NULL,
        CONSTRAINT [PK_AuditoriaSeguridad] PRIMARY KEY ([AUD_ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_USU_Nombre] ON [Usuarios] ([USU_Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    CREATE INDEX [IX_AuditoriaSeguridad_AUD_Fecha] ON [AuditoriaSeguridad] ([AUD_Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    ALTER TABLE [AuditoriaSesiones] ADD CONSTRAINT [FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID] FOREIGN KEY ([AS_UsuarioUSU_ID]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = 'Inactivo')
        INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES ('Inactivo');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    UPDATE AuditoriaSesiones SET AS_USU_ID = AS_UsuarioUSU_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004042402_SeguridadCredenciales'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261004042402_SeguridadCredenciales', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004192520_GestionGrupos'
)
BEGIN
    UPDATE Grupos SET GRU_Nombre = LTRIM(RTRIM(GRU_Nombre));
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004192520_GestionGrupos'
)
BEGIN

    UPDATE g
    SET GRU_Nombre = LEFT(g.GRU_Nombre, 60 - LEN(' (' + CAST(g.GRU_ID AS nvarchar(10)) + ')')) + ' (' + CAST(g.GRU_ID AS nvarchar(10)) + ')'
    FROM Grupos g
    WHERE EXISTS (SELECT 1 FROM Grupos o WHERE o.GRU_Nombre = g.GRU_Nombre AND o.GRU_ID < g.GRU_ID);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004192520_GestionGrupos'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM Estados_Grupos WHERE EST_GRU_Nombre = N'Activo')
        INSERT INTO Estados_Grupos (EST_GRU_Nombre) VALUES (N'Activo');
    IF NOT EXISTS (SELECT 1 FROM Estados_Grupos WHERE EST_GRU_Nombre = N'Inactivo')
        INSERT INTO Estados_Grupos (EST_GRU_Nombre) VALUES (N'Inactivo');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004192520_GestionGrupos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Grupos_GRU_Nombre] ON [Grupos] ([GRU_Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004192520_GestionGrupos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261004192520_GestionGrupos', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [AuditoriaSesiones] DROP CONSTRAINT [FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [Usuarios] DROP CONSTRAINT [FK_Usuarios_Personas_USU_PersonaPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    DROP INDEX [IX_Usuarios_USU_PersonaPER_ID] ON [Usuarios];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    DROP INDEX [IX_AuditoriaSesiones_AS_UsuarioUSU_ID] ON [AuditoriaSesiones];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    UPDATE Usuarios SET PER_ID = USU_PersonaPER_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    UPDATE AuditoriaSesiones SET AS_USU_ID = AS_UsuarioUSU_ID;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuditoriaSesiones]') AND [c].[name] = N'AS_UsuarioUSU_ID');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [AuditoriaSesiones] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [AuditoriaSesiones] DROP COLUMN [AS_UsuarioUSU_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'USU_PersonaPER_ID');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [Usuarios] DROP COLUMN [USU_PersonaPER_ID];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_Version] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [USU_UltimoAcceso] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    UPDATE Usuarios SET USU_Mail = LTRIM(RTRIM(USU_Mail));
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN

    UPDATE u
    SET USU_Mail = LEFT('duplicado-' + CAST(u.USU_ID AS nvarchar(10)) + '-' + u.USU_Mail, 60)
    FROM Usuarios u
    WHERE EXISTS (SELECT 1 FROM Usuarios o WHERE o.USU_Mail = u.USU_Mail AND o.USU_ID < u.USU_ID);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Activo')
        INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Activo');
    IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Inactivo')
        INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Inactivo');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    CREATE INDEX [IX_Usuarios_PER_ID] ON [Usuarios] ([PER_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_USU_Mail] ON [Usuarios] ([USU_Mail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    CREATE INDEX [IX_AuditoriaSesiones_AS_USU_ID] ON [AuditoriaSesiones] ([AS_USU_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [AuditoriaSesiones] ADD CONSTRAINT [FK_AuditoriaSesiones_Usuarios_AS_USU_ID] FOREIGN KEY ([AS_USU_ID]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Personas_PER_ID] FOREIGN KEY ([PER_ID]) REFERENCES [Personas] ([PER_ID]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004193506_GestionUsuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261004193506_GestionUsuarios', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    ALTER TABLE [Ventas] ADD [CAJ_ID] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE TABLE [Cajas] (
        [CAJ_ID] int NOT NULL IDENTITY,
        [CAJ_Estado] int NOT NULL,
        [CAJ_FechaApertura] datetime2 NOT NULL,
        [CAJ_UsuarioAperturaId] int NOT NULL,
        [CAJ_MontoInicial] decimal(18,2) NOT NULL,
        [CAJ_FechaCierre] datetime2 NULL,
        [CAJ_UsuarioCierreId] int NULL,
        [CAJ_CantidadVentas] int NULL,
        [CAJ_TotalVentas] decimal(18,2) NULL,
        [CAJ_EfectivoEsperado] decimal(18,2) NULL,
        [CAJ_EfectivoContado] decimal(18,2) NULL,
        [CAJ_Diferencia] decimal(18,2) NULL,
        [CAJ_Observaciones] nvarchar(500) NULL,
        CONSTRAINT [PK_Cajas] PRIMARY KEY ([CAJ_ID]),
        CONSTRAINT [CK_Cajas_Cierre] CHECK (([CAJ_Estado] = 0 AND [CAJ_FechaCierre] IS NULL AND [CAJ_UsuarioCierreId] IS NULL) OR ([CAJ_Estado] = 1 AND [CAJ_FechaCierre] IS NOT NULL AND [CAJ_UsuarioCierreId] IS NOT NULL AND [CAJ_EfectivoContado] IS NOT NULL)),
        CONSTRAINT [CK_Cajas_EfectivoContado] CHECK ([CAJ_EfectivoContado] IS NULL OR [CAJ_EfectivoContado] >= 0),
        CONSTRAINT [CK_Cajas_MontoInicial] CHECK ([CAJ_MontoInicial] >= 0),
        CONSTRAINT [FK_Cajas_Usuarios_CAJ_UsuarioAperturaId] FOREIGN KEY ([CAJ_UsuarioAperturaId]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cajas_Usuarios_CAJ_UsuarioCierreId] FOREIGN KEY ([CAJ_UsuarioCierreId]) REFERENCES [Usuarios] ([USU_ID]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE TABLE [CajaTotalesMedio] (
        [CTM_ID] int NOT NULL IDENTITY,
        [CAJ_ID] int NOT NULL,
        [MP_ID] int NOT NULL,
        [CTM_MedioPago] nvarchar(60) NOT NULL,
        [CTM_EsEfectivo] bit NOT NULL,
        [CTM_Cantidad] int NOT NULL,
        [CTM_Total] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_CajaTotalesMedio] PRIMARY KEY ([CTM_ID]),
        CONSTRAINT [FK_CajaTotalesMedio_Cajas_CAJ_ID] FOREIGN KEY ([CAJ_ID]) REFERENCES [Cajas] ([CAJ_ID]) ON DELETE CASCADE,
        CONSTRAINT [FK_CajaTotalesMedio_MetodosPago_MP_ID] FOREIGN KEY ([MP_ID]) REFERENCES [MetodosPago] ([MP_ID]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE INDEX [IX_Ventas_CAJ_ID] ON [Ventas] ([CAJ_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE INDEX [IX_Cajas_CAJ_FechaApertura] ON [Cajas] ([CAJ_FechaApertura]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE INDEX [IX_Cajas_CAJ_UsuarioAperturaId] ON [Cajas] ([CAJ_UsuarioAperturaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE INDEX [IX_Cajas_CAJ_UsuarioCierreId] ON [Cajas] ([CAJ_UsuarioCierreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Cajas_UnaAbierta] ON [Cajas] ([CAJ_Estado]) WHERE [CAJ_Estado] = 0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CajaTotalesMedio_CAJ_ID_MP_ID] ON [CajaTotalesMedio] ([CAJ_ID], [MP_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    CREATE INDEX [IX_CajaTotalesMedio_MP_ID] ON [CajaTotalesMedio] ([MP_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    ALTER TABLE [Ventas] ADD CONSTRAINT [FK_Ventas_Cajas_CAJ_ID] FOREIGN KEY ([CAJ_ID]) REFERENCES [Cajas] ([CAJ_ID]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN

    DECLARE @form int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarVentas');
    IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'AbrirCaja')
        INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'AbrirCaja', @form);
    IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'CerrarCaja')
        INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'CerrarCaja', @form);

    INSERT INTO AccionGrupo (AccionesACC_ID, GruposGRU_ID)
    SELECT nueva.ACC_ID, ag.GruposGRU_ID
    FROM AccionGrupo ag
    JOIN Acciones vender ON vender.ACC_ID = ag.AccionesACC_ID AND vender.ACC_Nombre = N'RealizarVenta'
    CROSS JOIN Acciones nueva
    WHERE nueva.ACC_Nombre IN (N'AbrirCaja', N'CerrarCaja')
      AND NOT EXISTS (SELECT 1 FROM AccionGrupo x WHERE x.AccionesACC_ID = nueva.ACC_ID AND x.GruposGRU_ID = ag.GruposGRU_ID);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012190029_ControlCaja'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261012190029_ControlCaja', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013133909_PermisoHistorialCajas'
)
BEGIN

    DECLARE @form int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarVentas');
    IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'VerHistorialCajas')
        INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'VerHistorialCajas', @form);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013133909_PermisoHistorialCajas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013133909_PermisoHistorialCajas', N'10.0.8');
END;

COMMIT;
GO

