# Guía de ABM maestros (Libros, Proveedores, Clientes)

Los tres ABM siguen el mismo patrón y comparten casi todo su comportamiento. Esta guía explica la estructura y cómo agregar un ABM nuevo (o un campo) sin duplicar código.

## Arquitectura

```
Vista (WinForms)                                   Controladora (negocio)                    Modelo
────────────────                                   ──────────────────────                    ──────
FrmGestionarX                                      XService : ServicioAbmBase, IXService      Entidad X (X_Activo, X_Version)
  └─ ListadoAbm<XListadoDTO>  ──ObtenerTodosAsync─▶  - DbContext propio por operación         XListadoDTO / XEdicionDTO / FiltroX
FrmEditarX(id?)                                      - AsNoTracking + Select a DTO            Excepciones de dominio
  └─ EdicionAbm<XEdicionDTO>  ──GuardarAsync──────▶  - Validación completa + unicidad          Identificadores (CUIT, DNI, ISBN,
       + ValidadorFormulario (ErrorProvider)          - Baja lógica y reactivación              email, teléfono)
                                                     - Concurrencia optimista (X_Version)
                                                     - Errores de EF/SQL → mensajes claros
```

Reglas:

- **Los formularios no usan `DbContext` ni LINQ a la base.** Solo llaman al servicio (a través de su interfaz) y muestran DTOs.
- **Las lecturas** son `AsNoTracking()` + `Select(...)` a un DTO: una consulta SQL, sin cargar entidades relacionadas (sin N+1).
- **La validación vive en el servicio.** La UI repite las de formato solo para dar feedback inmediato; si la UI falla, el servicio igual rechaza.
- **No hay borrado físico.** `CambiarEstadoAsync(id, false)` da de baja; el registro conserva su historial (ventas, órdenes, precios).
- **Identificador único que ya existe pero está inactivo:** el servicio lanza `EntidadInactivaException` y el modal ofrece reactivar ese registro en lugar de duplicarlo.

## Experiencia de usuario (igual en los tres)

| Acción | Atajo |
|---|---|
| Nuevo | **F2** |
| Modificar | **F3**, Enter o doble clic |
| Dar de baja / Reactivar | **F4** o Supr |
| Refrescar | F5 |
| Imprimir | Ctrl+P |

- **Búsqueda en tiempo real:** espera 300 ms sin tipear y cancela la consulta anterior.
- **Filtro de estado:** Todos, Solo activos (por defecto) e Inactivos. Cada entidad agrega un filtro propio: género, condición fiscal o localidad.
- **Grilla:** columnas explícitas y formateadas; los inactivos se ven en gris y en cursiva.
- **Modal único** para alta y edición:
  - Título dinámico: "Nuevo proveedor" o "Editar proveedor: Razón social".
  - `ErrorProvider` en cada campo, con validación del identificador al salir del campo (formato y si ya existe).
  - Aviso si cerrás con cambios sin guardar.
  - Cierra con `DialogResult.OK` y el listado se recarga seleccionando el registro guardado.

## Piezas compartidas

| Pieza | Dónde | Para qué |
|---|---|---|
| `IServicioAbm<TListado, TEdicion, TFiltro>` e `ILibroService` / `IProveedorService` / `IClienteService` | `Controladora/Abm/IServicioAbm.cs` | Contratos (`ObtenerTodosAsync`, `ObtenerPorIdAsync`, `GuardarAsync`, `CambiarEstadoAsync`, `ExisteIdentificadorAsync`, `ReactivarAsync`) |
| `ServicioAbmBase` | `Controladora/Abm` | Acumulador de errores por campo y `GuardarCambiosAsync`, que traduce concurrencia, clave duplicada, FK, deadlock y timeout |
| `FiltroAbm`, `FiltroEstadoActivo`, `Identificadores` | `Modelo/AbmDTOs.cs` | Filtros base y validadores de CUIT, DNI, ISBN, email y teléfono (los usan UI y servicio) |
| Excepciones de dominio | `Modelo/Excepciones.cs` | `ValidacionException`, `EntidadInactivaException`, `ConcurrenciaException`, `PersistenciaException` |
| `ListadoAbm<T>` | `Vista/Comun` | Todo el comportamiento del formulario de gestión |
| `EdicionAbm<T>` | `Vista/Comun` | Todo el ciclo de vida del modal (carga, guardado, reactivación, concurrencia, cambios sin guardar) |
| `ValidadorFormulario` | `Vista/Comun` | `ErrorProvider` + mapa "campo del DTO → control" para mostrar los errores del servicio |
| `GrillaHelper`, `ExportadorGrilla`, `ImpresorGrilla`, `ManejadorErrores` | `Vista/Comun` | Columnas, exportar a Excel, imprimir con vista previa y mostrar o registrar errores |

## Agregar un ABM nuevo

1. **Entidad:**
   - Agregar `X_Activo` (`bool`, por defecto `true`) y `X_Version` (`[ConcurrencyCheck] int`).
   - Si tiene identificador único, declarar en `Libreria.OnModelCreating` un índice único filtrado (`IS NOT NULL`).
2. **DTOs:** `FiltroX : FiltroAbm`, `XListadoDTO` y `XEdicionDTO` (con `Id?` y `Version`).
3. **Servicio:** `XService : ServicioAbmBase, IXService`. Copiar `ProveedorService` como plantilla y adaptar `Normalizar`, `ValidarAsync` y el mapeo DTO ↔ entidad.
4. **Gestión:**
   - Generar el diseñador desde la misma plantilla que usan los tres listados (mismos nombres de controles: `dgvListado`, `txtBuscar`, `cbEstado`, `cbFiltroExtra`...).
   - En el código: definir las columnas con `GrillaHelper` y crear el `ListadoAbm<XListadoDTO>` indicando qué servicio llamar.
5. **Modal:** diseñador con `ErrorProvider`. En el código: el mapa del `ValidadorFormulario`, `Mostrar`, `Armar` y `ValidarFormulario`, más el `EdicionAbm<XEdicionDTO>`.
6. **Migración:** revisarla siempre a mano. Hay que confirmar tres cosas:
   - **Valores por defecto:** el campo activo tiene que tener `defaultValue: true`. EF genera `false`, y eso deja a todos los registros existentes inactivos.
   - **Duplicados:** limpiarlos antes de crear un índice único.
   - **Renombres:** que EF no haya convertido una columna existente en otra. En `AbmProveedoresClientes` intentó renombrar las FK sombra a `PROV_Version`/`CLI_Version`, y eso habría perdido los vínculos con `Persona`.

## Particularidades

- **Proveedores:**
  - El CUIT es obligatorio y único; se guarda solo con dígitos y se muestra como `30-12345678-9`.
  - El contacto (nombre, teléfono, email) vive en `Persona`.
  - Los proveedores inactivos no se ofrecen en fichas de libro ni en órdenes nuevas, y no cuentan como "proveedor habitual".
- **Clientes:**
  - El documento puede ser DNI (7-8 dígitos) o CUIT (11 dígitos) y es único.
  - `PER_DNI` se mantiene sincronizado para las pantallas que buscan por DNI.
  - "Consumidor Final" (`CLI_ConsumidorFinal`) es de sistema: no se edita ni se da de baja, y el punto de venta lo usa por defecto.
  - Los clientes inactivos no aparecen en el punto de venta.
- **Libros:** ISBN opcional pero único, margen positivo y el stock solo se carga en el alta.

### Datos existentes al migrar

- Los **proveedores existentes** quedan sin CUIT (antes no se guardaba). El ABM lo exige la próxima vez que se editen.
- Los **clientes** toman como documento su DNI. Si el DNI estaba repetido, solo el de menor ID lo conserva; los demás deben completarlo al editarlos.
