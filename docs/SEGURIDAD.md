# Módulo de seguridad: auditoría y guía

Estado del módulo de seguridad (login, usuarios, grupos, permisos y sesión): qué se encontró, qué se corrigió y qué queda pendiente. Sirve también como checklist para cualquier cambio futuro.

## Arquitectura

```
FrmIniciarSesión ──IniciarSesion──▶ ControladoraUsuarios ──▶ HasherClaves (PBKDF2)       Usuario ─┬─ Grupos ── Acciones ── Formulario ── Modulo
      │                               ├─ bloqueo por intentos                                     └─ Acciones (directas)
      │ ResultadoLogin                ├─ clave temporal / cambio obligatorio
      ▼                               └─ BitacoraSeguridad ──▶ AuditoriaSeguridad (tabla)
FrmCambiarClave (si DebeCambiarClave)
      ▼
Sesion.Usuario + PermisoService ◀── TienePermiso (UI: ocultar)   /   Exigir (negocio: lanza AccesoDenegadoException)
      ▼
FrmMenu ── Navegar<T>() verifica PuedeAccederFormulario
```

El modelo ya es **RBAC con permisos directos**: un usuario recibe acciones por sus grupos y, además, acciones sueltas. Cada acción puede pertenecer a un formulario, que es lo que habilita la entrada del menú.

## Hallazgos

Severidad: 🔴 crítica · 🟠 alta · 🟡 media.

| # | Sev. | Hallazgo | Estado |
|---|---|---|---|
| 1 | 🔴 | Claves con **SHA-256 sin salt** (un solo cálculo, rompible con diccionario o tablas precalculadas; dos usuarios con la misma clave tenían el mismo hash). | ✅ PBKDF2-HMAC-SHA256 con 600.000 iteraciones y salt aleatorio. Los hashes viejos se migran solos en el próximo login. |
| 2 | 🔴 | Claves temporales de **5 dígitos con `Random`** y `Next(0, 9)`, que nunca genera el 9: solo 59.049 combinaciones posibles. | ✅ 10 caracteres con `RandomNumberGenerator` (~57 bits de entropía). |
| 3 | 🔴 | **Sin bloqueo por intentos fallidos**: fuerza bruta ilimitada. | ✅ 5 intentos → 15 minutos de bloqueo. El reseteo de un administrador desbloquea. |
| 4 | 🔴 | **Recuperación de clave sin verificación**: con solo saber el usuario y el email de otro se le cambiaba la clave y quedaba sin acceso. | ✅ La clave temporal convive con la actual, vence a los 30 minutos y se invalida con cualquier login correcto. |
| 5 | 🔴 | **Escalada de privilegios renombrando un grupo**: ser administrador depende de que el grupo se llame "Administrador", y `ModificarGrupo` no validaba nombres. Quien pudiera editar grupos podía dar acceso total a cualquier grupo. | ✅ El grupo Administrador no se renombra, no se deshabilita ni se elimina, y ningún otro grupo puede tomar ese nombre (además, índice único en `GRU_Nombre`). Un grupo deshabilitado ya no otorga permisos de administrador. |
| 6 | 🔴 | **Borrar un usuario borraba su historial de sesiones** (FK en cascada). | ✅ Baja lógica (estado "Inactivo") y FK `Restrict`. |
| 7 | 🟠 | El **hash se mostraba en la grilla** de usuarios (`UsuarioDTO.Clave`) y quedaba en memoria toda la sesión. | ✅ Se quitó del DTO. Los usuarios que devuelve la controladora salen sin hashes. |
| 8 | 🟠 | Las **altas y los reseteos no exigían cambiar la clave** temporal recibida por email. | ✅ `USU_DebeCambiarClave`: no se entra sin elegir una clave propia. |
| 9 | 🟠 | **Autorización solo en la UI**: los métodos de negocio no verificaban permisos. | 🟡 Parcial: usuarios y grupos ya llaman a `PermisoService.Exigir(...)`. Falta en ventas, inventario y el resto (ver pendientes). |
| 10 | 🟠 | Un usuario con permiso para editar usuarios o grupos podía **ampliarse sus propios permisos**. | ✅ Nadie modifica sus propios grupos o acciones, ni las acciones de un grupo al que pertenece. |
| 11 | 🟠 | **Sin auditoría** de logins fallidos ni de cambios de permisos. | ✅ Tabla `AuditoriaSeguridad`: logins (correctos, fallidos, bloqueos), recuperación, reseteo, cambio de clave y altas, cambios y bajas de usuarios y grupos (con la lista de permisos). |
| 12 | 🟡 | `AuditoriaSesiones.AS_USU_ID` guardaba el **ID de la persona** en lugar del ID del usuario. | ✅ Corregido, y la migración arregla los registros existentes. |
| 13 | 🟡 | El estado "Inactivo" **no existía en la base**: no había forma de inhabilitar un usuario. | ✅ La migración lo crea. |
| 14 | 🟡 | El nombre de usuario **no era único**, aunque el login busca por nombre. | ✅ Índice único y validación en el alta y la modificación. |
| 15 | 🟡 | `FrmCambiarClave` mostraba las claves en texto visible y comparaba contra el hash guardado en memoria. | ✅ Campos con `UseSystemPasswordChar`. La verificación se hace en la controladora. |
| 16 | 🟡 | El texto de ayuda "CONTRASEÑA" se enviaba como clave y sumaba intentos fallidos. | ✅ Se filtra antes de llamar al login. |
| 17 | 🟡 | Los mensajes de recuperación y login revelaban qué usuarios existen. | ✅ Mensajes genéricos. El login con un usuario inexistente tarda lo mismo que con uno real. |
| 18 | 🟡 | "Grupo activo" se decidía de tres formas distintas: `EST_GRU_ID == 1` (login y `PermisoService`) y un estado "Deshabilitado" que no existía (`Usuario.getAllGruposActivos`). No había forma de deshabilitar un grupo. | ✅ `Grupo.EstaActivo` (por nombre: `Estado_Grupo.Activo`) en todos lados, y la migración `GestionGrupos` crea el estado "Inactivo". |
| 19 | 🟡 | En `FrmGrupo`, marcar un módulo o formulario en el árbol **no guardaba nada** (sin `AfterCheck`; se leían solo las hojas). Eliminar un grupo no pedía confirmación. | ✅ Gestión de grupos master-detail sobre `GrupoService` (ver abajo). |
| 20 | 🟡 | La grilla de usuarios no mostraba los grupos ni el último acceso, no había forma de desbloquear ni de reactivar desde el listado, y el alta se rechazaba entera si el email no salía. | ✅ Gestión de usuarios sobre `UsuarioService` (ver abajo). |
| 21 | 🟡 | La FK real de Usuario → Persona era la columna sombra `USU_PersonaPER_ID` (`PER_ID` podía valer 0), y `AuditoriaSesiones` guardaba el usuario dos veces (`AS_USU_ID` y la sombra `AS_UsuarioUSU_ID`). El email no era único en la base. | ✅ Migración `GestionUsuarios`: `PER_ID` y `AS_USU_ID` son las FK reales e índice único en `USU_Mail`. |
| — | ✅ | **Inyección SQL**: todo pasa por LINQ (parametrizado). El único SQL manual (`FromSqlInterpolated` en `OrdenReposicionService`) también se parametriza. | Sin cambios. |

## Checklist para cualquier cambio de seguridad

**Credenciales**
- [x] Nunca texto plano ni MD5/SHA1/SHA-256 "pelado": solo `HasherClaves.Hashear` / `Verificar`.
- [x] Comparaciones de hash en tiempo constante (`CryptographicOperations.FixedTimeEquals`).
- [x] Política de claves en la capa de negocio (`PoliticaClaves`): 8 caracteres o más, letras y números, sin contener el nombre de usuario.
- [x] Bloqueo temporal por intentos fallidos.
- [x] Claves temporales con un RNG criptográfico, con vencimiento y cambio obligatorio.
- [x] Las claves no se loguean, no viajan en DTOs ni quedan en la sesión.

**Autorización**
- [x] UI: ocultar o deshabilitar con `PermisoService.TienePermiso` / `PuedeAccederFormulario`.
- [ ] Negocio: **todo** método que modifica datos empieza con `PermisoService.Instancia.Exigir("Accion")`. Hecho en usuarios y grupos.
- [x] Sin autoasignación de permisos. El grupo Administrador está protegido.

**Sesión**
- [x] En memoria: ID, nombre, persona, grupos y acciones. Nunca hashes ni claves.
- [x] Logout: `RegistrarLogout` + `PermisoService.Logout()` + `Sesion.Usuario = null`, y se libera el menú con sus secciones (`using` en el login).
- [x] Cierre por inactividad (15 minutos) que cierra los modales y vuelve al login.

**Persistencia y auditoría**
- [x] Solo LINQ o SQL interpolado parametrizado. Nunca concatenar strings en SQL.
- [x] Baja lógica de usuarios. El historial no se borra en cascada.
- [x] Auditoría con fecha, usuario, evento, detalle y equipo. Guarda el nombre (no una FK) para sobrevivir a cambios y bajas.

## Cómo verificar un permiso

```csharp
// UI: comodidad (que el usuario no vea lo que no puede usar)
btnAnular.Visible = PermisoService.Instancia.TienePermiso("AnularVenta");

// Negocio: seguridad (aunque alguien llegue al método por otro camino)
public void AnularVenta(int ventaId, string motivo)
{
    PermisoService.Instancia.Exigir("AnularVenta");   // lanza AccesoDenegadoException
    ...
}
```

`ManejadorErrores.Mostrar` muestra el mensaje de `AccesoDenegadoException` sin datos técnicos. El menú (`FrmMenu`) aplica la misma idea: `AplicarSeguridad()` oculta los botones y `Navegar<T>()` vuelve a verificar el permiso antes de abrir la sección.

## Pendientes

1. **`Exigir` en el resto de la capa de negocio**: `FacadeVentas` (realizar y anular venta), `InventarioService`, `OrdenReposicionService` y los servicios ABM de libros, clientes y proveedores, usando los mismos nombres de acción que ya usa la UI.
2. **Usuario de auditoría tomado de la sesión, no de la UI**: hoy `VEN_Usuario`, `MOV_Usuario` y `OR_Usuario` llegan en la solicitud que arma el formulario. Deberían leerse de `Sesion.Instancia.Usuario` en la capa de negocio.
3. **Acceso directo a la base (arquitectura de 2 capas)**: cada PC se conecta a SQL Server con su usuario de Windows (`Integrated Security`). Cualquiera con esos permisos puede abrir SSMS y saltear toda la seguridad de la aplicación, por ejemplo con un `UPDATE` sobre `Usuarios`. Mitigaciones, de menor a mayor esfuerzo:
   - Un login SQL de mínimo privilegio solo para la app (sin `db_owner`) y que los usuarios de Windows no tengan acceso a la base.
   - Permisos por tabla o procedimientos almacenados para las operaciones sensibles.
   - A futuro, una API intermedia (3 capas), para que el cliente nunca tenga credenciales de la base.
4. ~~**Proteger al último administrador**~~ ✅ Hecho en `UsuarioService`: no se puede dar de baja ni quitar del grupo al último administrador activo.
5. **Límite de solicitudes de recuperación** (por ejemplo, una cada 5 minutos por usuario) para evitar inundar casillas de email.
6. ~~**Acceso voluntario a "Cambiar clave"**~~ ✅ Botón "Mi clave" en el menú.
7. **Pantalla de consulta de `AuditoriaSeguridad`** con filtros por fecha, usuario y evento.
8. **Marca de administrador**: reemplazar el chequeo por nombre de grupo por una acción explícita (por ejemplo, `AdministrarSistema`) o una columna `GRU_EsAdministrador`.

## Migración

`20261004042402_SeguridadCredenciales`:
- Amplía `USU_Clave` a 200 caracteres.
- Agrega las columnas de bloqueo, de cambio obligatorio y de clave temporal.
- Crea `AuditoriaSeguridad`.
- Agrega el índice único en `USU_Nombre` y cambia la FK de sesiones a `Restrict`.
- Crea el estado "Inactivo" y corrige `AS_USU_ID`.

**No invalida ninguna clave**: los usuarios entran con la clave de siempre y el hash se convierte a PBKDF2 en ese mismo login.

```bash
dotnet ef database update --project Modelo --startup-project Vista
```

`20261004192520_GestionGrupos`:
- Recorta espacios y depura nombres de grupo duplicados (el de menor ID conserva el nombre; los demás quedan como `Nombre (ID)`).
- Crea los estados de grupo "Activo" e "Inactivo" si faltan.
- Agrega el índice único en `GRU_Nombre`.

`20261004193506_GestionUsuarios`:
- `PER_ID` pasa a ser la FK real hacia `Personas` y `AS_USU_ID` hacia `Usuarios` (se copian las columnas sombra antes de borrarlas; la de sesiones sigue en `Restrict`).
- Agrega `USU_UltimoAcceso` y `USU_Version` (concurrencia optimista).
- Depura emails duplicados y agrega el índice único en `USU_Mail`.

## Gestión de grupos

`FrmGestionarGrupos` es master-detail (listado con búsqueda en vivo a la izquierda; ficha con árbol de permisos y usuarios del grupo a la derecha) y no usa `DbContext`: todo pasa por `GrupoService` (`Controladora/Seguridad`), que concentra las reglas:

- `PermisoService.Exigir` en alta (`AgregarGrupo`), modificación (`ModificarGrupo`) y eliminación (`EliminarGrupo`).
- Grupo Administrador: no se renombra, no se deshabilita, no se elimina y su nombre está reservado; sólo se edita la descripción.
- Nadie cambia las acciones de un grupo al que pertenece (la ficha muestra el árbol en sólo lectura y el servicio lo rechaza).
- Nombre único en alta y modificación; las acciones se sincronizan por ID (sólo cambian las filas de `AccionGrupo` que difieren).
- Eliminar sólo grupos sin usuarios, en una transacción serializable; si tiene usuarios, se propone deshabilitarlo.
- Altas, cambios y bajas quedan en `AuditoriaSeguridad`.

El árbol (`Vista/Comun/ArbolPermisos`) guarda la selección en un `HashSet` (filtrar o recargar no la pierde), propaga padre/hijos, muestra el conteo por nodo y corrige el doble clic del TreeView nativo.

## Gestión de usuarios

`FrmGestionarUsuarios` usa `ListadoAbm` (búsqueda en vivo, filtros por estado, grupo y "sólo bloqueados", F2/F3/F4, exportar e imprimir) y `FrmEditarUsuario` usa `EdicionAbm` (un único modal de alta y edición, con `ErrorProvider`). Ninguno usa `DbContext`: todo pasa por `UsuarioService` (`Controladora/Seguridad`). El login, el cambio de clave propio y la recuperación siguen en `ControladoraUsuarios`.

- La grilla nunca recibe datos de la clave. Columnas: usuario, nombre, email, grupos (concatenados en SQL con `STRING_AGG`), estado (activo, inactivo, bloqueado o con clave temporal) y último acceso.
- La ficha no tiene campo de clave: en el alta la genera el servicio y se envía por email **después** de guardar. Si el email no sale, la cuenta igual se crea y la clave temporal se muestra una única vez al operador (`FrmClaveTemporal`) para entregarla en mano.
- El nombre de usuario no se edita después del alta (ventas, movimientos y órdenes lo guardan como texto).
- Permisos heredados de los grupos en gris en el árbol (no se pueden quitar desde la ficha); los directos se marcan aparte.
- `PermisoService.Exigir` en alta, edición, baja/reactivación (`EliminarUsuario`), reseteo (`ResetearClave`) y desbloqueo (`ModificarUsuario`).
- Nadie cambia sus propios grupos o permisos ni se da de baja; siempre queda al menos un administrador activo.
- Baja lógica con reactivación (un alta que coincide con una cuenta dada de baja ofrece reactivarla).
- Concurrencia optimista con `USU_Version`.
- Todo cambio queda en `AuditoriaSeguridad` (incluye `USUARIO_REACTIVADO` y `USUARIO_DESBLOQUEADO`).
