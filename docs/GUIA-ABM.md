# Guía de ABM maestros (Libros, Proveedores, Clientes)

Libros es la implementación de referencia. Esta guía explica el patrón y cómo replicarlo en Proveedores y Clientes sin duplicar código.

## Arquitectura

```
Vista (WinForms)                         Controladora (negocio)                 Modelo
─────────────────                        ──────────────────────                 ──────
FrmGestionarX  ──ObtenerTodosAsync──▶    XService : ServicioAbmBase,             Entidad X (con X_Activo, X_Version)
  (grilla + filtros + acciones)            IServicioAbm<XListadoDTO,             XListadoDTO / XEdicionDTO / FiltroX
FrmEditarX(id?) ──GuardarAsync──────▶      XEdicionDTO, FiltroX>                 ValidacionException, EntidadInactivaException,
  (ErrorProvider + ValidadorFormulario)   - DbContext propio por operación        ConcurrenciaException, PersistenciaException
                                          - AsNoTracking + Select a DTO
Vista/Comun (compartido)                  - Validación completa + unicidad
  GrillaHelper, ExportadorGrilla,         - Baja lógica y reactivación
  ValidadorFormulario, ManejadorErrores   - Traducción de errores de EF/SQL
```

Reglas:

- **Los formularios no usan `DbContext` ni LINQ a la base.** Sólo llaman al servicio y muestran DTOs.
- **Las lecturas** son `AsNoTracking()` + `Select(...)` a un DTO: una consulta SQL, sin entidades relacionadas cargadas (sin N+1).
- **La validación vive en el servicio.** La UI repite las de formato sólo para dar feedback inmediato; si la UI se equivoca, el servicio igual rechaza.
- **No hay borrado físico.** `CambiarEstadoAsync(id, false)` desactiva; el registro sigue existiendo para ventas, órdenes e historial.
- **Mensajes limpios.** El servicio lanza excepciones de dominio con texto para el usuario; `ManejadorErrores.Mostrar` decide cómo mostrarlas y registra en log las inesperadas.

## Piezas compartidas (no se duplican)

| Pieza | Dónde | Para qué |
|---|---|---|
| `IServicioAbm<TListado, TEdicion, TFiltro>` | `Controladora/Abm` | Contrato CRUD estándar |
| `ServicioAbmBase` | `Controladora/Abm` | `Errores` (acumulador de validaciones) y `GuardarCambiosAsync` (traduce concurrencia, índice único, FK, deadlock) |
| `FiltroAbm`, `FiltroEstadoActivo` | `Modelo/AbmDTOs.cs` | Texto + Activos/Inactivos/Todos |
| `Identificadores` | `Modelo/AbmDTOs.cs` | `EsISBNValido`, `EsCuitValido`, `EsEmailValido` (los usan UI y servicio) |
| Excepciones de dominio | `Modelo/Excepciones.cs` | `ValidacionException` (errores por campo), `EntidadInactivaException`, `ConcurrenciaException`, `PersistenciaException` |
| `GrillaHelper` | `Vista/Comun` | Columnas explícitas, doble buffer, re-selección tras recargar |
| `ExportadorGrilla` | `Vista/Comun` | Exporta cualquier grilla a CSV (Excel) con las columnas visibles |
| `ValidadorFormulario` | `Vista/Comun` | `ErrorProvider` + mapa campo del DTO → control |
| `ManejadorErrores` | `Vista/Comun` | Mensaje según el tipo de excepción + log en `%LOCALAPPDATA%\Scriptum\errores.log` |

## Pasos para replicar (ej. Proveedores)

1. **Entidad** (`Modelo/Proveedor.cs`): agregar `PROV_Activo` (`bool`, por defecto `true`), `PROV_Version` (`[ConcurrencyCheck] int`) y los campos nuevos (`PROV_CUIT`, `PROV_Direccion`, `PROV_CondicionFiscal`). En `Libreria.OnModelCreating`, índice único filtrado sobre `PROV_CUIT`.
2. **Migración**: `dotnet ef migrations add AbmProveedores --project Modelo --startup-project Vista`. Revisarla siempre a mano:
   - El `defaultValue` del campo activo tiene que ser `true`. EF genera `false`, y eso deja a todos los registros existentes inactivos.
   - Limpiar duplicados antes de crear el índice único (ver la migración `AbmLibros`).
3. **DTOs** (en `Modelo/AbmDTOs.cs`): `FiltroProveedores : FiltroAbm` (más `CondicionFiscal?`), `ProveedorListadoDTO`, `ProveedorEdicionDTO` (con `Id?` y `Version`).
4. **Servicio** (`Controladora/Abm/ProveedorService.cs`): copiar `LibroService` y adaptar:
   - `NombreEntidad => "el proveedor"`.
   - `ValidarAsync`: Razón social, Contacto y Teléfono obligatorios. `Identificadores.EsCuitValido`. Email con `Identificadores.EsEmailValido`. Unicidad de CUIT: si existe inactivo, lanzar `EntidadInactivaException`.
   - `GuardarCambiosAsync(db, ct, campoUnico: nameof(ProveedorEdicionDTO.CUIT), etiquetaUnico: "CUIT")`.
5. **Formularios**: copiar `FrmGestionarLibros` y `FrmEditarLibro`, cambiar columnas, filtros, campos y el mapa del `ValidadorFormulario`. La estructura de eventos (carga cancelable, debounce, alta/edición, cambio de estado, exportar, reactivación, concurrencia, cambios sin guardar) se mantiene igual.
6. **Consumidores**: donde se listan proveedores para elegir (combo de órdenes de reposición, ficha de libro), filtrar `PROV_Activo`.
7. **Retirar** `ControladoraProveedores`, `FrmAMProveedor` y el `FrmGestionarProveedores` viejo cuando ya nadie los use.

## Particularidades por entidad

### Proveedores
- **Identificador único:** CUIT (11 dígitos con dígito verificador). Guardarlo normalizado, solo dígitos.
- **Condición fiscal:** conviene un combo con valores fijos (Responsable Inscripto, Monotributo, Exento, Consumidor Final). Puede ser un `enum` guardado como texto.
- **Datos personales:** hoy el contacto vive en `Persona` (`PER_Proveedor`). Mantenerlo así: el servicio crea o actualiza la `Persona` dentro de la misma transacción.
- **Desactivar:** si el proveedor tiene órdenes activas, avisarlo en la confirmación. Las órdenes ya emitidas siguen su curso.

### Clientes
- **Identificador único:** DNI (`PER_DNI`). Excluir "Consumidor Final" (DNI 0) de la regla de unicidad y no permitir desactivarlo.
- **Email:** si se carga, validarlo con `Identificadores.EsEmailValido`.
- **Límite de crédito:** `decimal(18,2)`, mayor o igual a 0. Solo tiene efecto si más adelante se habilita la venta a cuenta corriente.
- **Desactivar:** el punto de venta debe ofrecer solo clientes activos.

## Pendientes del modelo actual a tener en cuenta

- **Claves foráneas sombra:** `Cliente.PER_ID` y `Proveedor.PER_ID` no son las FK reales; EF usa columnas sombra (`CLI_PersonaPER_ID`, `PER_ProveedorPER_ID`). Conviene corregirlas igual que se hizo con `Venta`, en la misma migración del ABM.
- **Identificador de cliente en los DTOs:** `ClienteDTO.CLIDTO_ID` contiene el `PER_ID`, no el `CLI_ID`. En los DTOs nuevos usar el `CLI_ID` real.
