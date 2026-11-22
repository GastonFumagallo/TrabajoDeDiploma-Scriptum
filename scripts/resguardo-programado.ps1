<#
.SYNOPSIS
    Resguardo completo y verificado de la base de SCRIPTUM, pensado para el Programador de tareas de Windows
    (SQL Server Express no incluye el Agente de SQL Server).

.DESCRIPTION
    1. Hace BACKUP DATABASE ... WITH CHECKSUM en la carpeta indicada, con un nombre único por fecha y hora.
    2. Verifica el archivo con RESTORE VERIFYONLY WITH CHECKSUM.
    3. Borra los resguardos de la base con más de -DiasRetencion días (por defecto, 30).
    4. Deja una línea por ejecución en resguardos.log, en la misma carpeta.
    Devuelve código de salida 1 si el resguardo o la verificación fallan, para que el Programador de tareas lo marque.

.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File resguardo-programado.ps1 -Servidor LAPTOP-TUCJ6TOI -Base Libreria -Carpeta "D:\Resguardos"

    Programar todos los días a las 21:00:
    schtasks /Create /TN "SCRIPTUM - Resguardo diario" /SC DAILY /ST 21:00 /RU SYSTEM /TR "powershell -NoProfile -ExecutionPolicy Bypass -File C:\SCRIPTUM\scripts\resguardo-programado.ps1 -Servidor . -Base Libreria -Carpeta D:\Resguardos"
#>
param(
    [string]$Servidor = '.',
    [string]$Base = 'Libreria',
    [Parameter(Mandatory = $true)][string]$Carpeta,
    [int]$DiasRetencion = 30
)

$ErrorActionPreference = 'Stop'
$fecha = Get-Date
$archivo = Join-Path $Carpeta ("{0}_{1:yyyyMMdd_HHmmss}.bak" -f $Base, $fecha)
$log = Join-Path $Carpeta 'resguardos.log'

function Escribir-Log([string]$mensaje) {
    $linea = "{0:yyyy-MM-dd HH:mm:ss}  {1}" -f (Get-Date), $mensaje
    Add-Content -Path $log -Value $linea -Encoding UTF8
    Write-Output $linea
}

try {
    if (-not (Test-Path $Carpeta)) { New-Item -ItemType Directory -Path $Carpeta | Out-Null }
    $baseSql = $Base.Replace(']', ']]')
    $archivoSql = $archivo.Replace("'", "''")

    & sqlcmd -S $Servidor -E -C -b -Q "BACKUP DATABASE [$baseSql] TO DISK = N'$archivoSql' WITH CHECKSUM, NAME = N'$Base - resguardo programado', DESCRIPTION = N'SCRIPTUM · usuario: tarea programada'"
    if ($LASTEXITCODE -ne 0) { throw "BACKUP DATABASE terminó con código $LASTEXITCODE." }

    & sqlcmd -S $Servidor -E -C -b -Q "RESTORE VERIFYONLY FROM DISK = N'$archivoSql' WITH CHECKSUM"
    if ($LASTEXITCODE -ne 0) { throw "La verificación del resguardo falló (código $LASTEXITCODE)." }

    $tamanio = [math]::Round((Get-Item $archivo).Length / 1MB, 2)
    Escribir-Log "OK  $archivo ($tamanio MB) verificado."

    $limite = $fecha.AddDays(-$DiasRetencion)
    Get-ChildItem -Path $Carpeta -Filter "$Base`_*.bak" |
        Where-Object { $_.LastWriteTime -lt $limite } |
        ForEach-Object { Remove-Item $_.FullName -Force; Escribir-Log "Eliminado por retención ($DiasRetencion días): $($_.Name)" }
    exit 0
}
catch {
    Escribir-Log "ERROR  $($_.Exception.Message)"
    exit 1
}
