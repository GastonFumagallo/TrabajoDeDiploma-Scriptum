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
| 5 | 🔴 | **Escalada de privilegios renombrando un grupo**: ser administrador depende de que el grupo se llame "Administrador", y `ModificarGrupo` no validaba nombres. Quien pudiera editar grupos podía dar acceso total a cualquier grupo. | ✅ El grupo Administrador no se renombra, no se deshabilita ni se elimina, y ningún otro grupo puede tomar ese nombre. Un grupo deshabilitado ya no otorga permisos de administrador. |
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
4. **Proteger al último administrador**: impedir dar de baja o quitar del grupo al último usuario administrador activo.
5. **Límite de solicitudes de recuperación** (por ejemplo, una cada 5 minutos por usuario) para evitar inundar casillas de email.
6. **Acceso voluntario a "Cambiar clave"** desde el menú. `FrmCambiarClave` ya lo soporta con `new FrmCambiarClave(Sesion.Instancia.Usuario.USU_ID)`.
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
