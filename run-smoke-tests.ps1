<#
.SYNOPSIS
    Script de ejecucion rapida para la suite de pruebas Smoke de PinBox (Seccion Amarilla).

.DESCRIPTION
    Ejecuta las pruebas de humo en modo Headless (sin interfaz grafica), genera un
    reporte interactivo en formato HTML dentro de 'TestResults/' y lo abre automaticamente
    en el navegador predeterminado al concluir.

.PARAMETER Filter
    Filtro de NUnit para seleccionar las pruebas. Por defecto: "Category=Smoke".

.PARAMETER ReportName
    Nombre del archivo de reporte HTML resultante. Por defecto: "Reporte_Smoke.html".

.PARAMETER NoOpenReport
    Si se especifica, evita abrir automaticamente el reporte HTML al finalizar (ideal para CI/CD).

.EXAMPLE
    .\run-smoke-tests.ps1
    Ejecuta todas las pruebas categorizadas como Smoke en modo Headless y abre el reporte.

.EXAMPLE
    .\run-smoke-tests.ps1 -Filter "Category=Smoke | Category=Sanity"
    Ejecuta pruebas de Smoke y Sanity combinadas.
#>

[CmdletBinding()]
param(
    [string]$Filter = "Category=Smoke",
    [string]$ReportName = "Reporte_Smoke.html",
    [switch]$NoOpenReport
)

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "==========================================================================" -ForegroundColor Cyan
Write-Host "   PINBOX E2E - SUITE DE SMOKE TESTS (SECCION AMARILLA / ADN DIGITAL)    " -ForegroundColor Yellow
Write-Host "==========================================================================" -ForegroundColor Cyan
Write-Host " [i] Modo de ejecucion : Headless (Navegador en segundo plano)" -ForegroundColor Gray
Write-Host " [i] Filtro aplicado   : $Filter" -ForegroundColor Gray
Write-Host " [i] Reporte esperado  : TestResults\$ReportName" -ForegroundColor Gray
Write-Host "--------------------------------------------------------------------------" -ForegroundColor DarkGray

# 1. Forzar modo Headless (asegurando que HEADED no este activo)
$env:HEADED = "0"
Remove-Item env:PLAYWRIGHT_SLOWMO -ErrorAction SilentlyContinue

# 2. Asegurar que exista la carpeta TestResults
$projectRoot = $PSScriptRoot
if (-not $projectRoot) {
    $projectRoot = Get-Location
}
$testResultsDir = Join-Path $projectRoot "TestResults"
if (-not (Test-Path $testResultsDir)) {
    New-Item -ItemType Directory -Path $testResultsDir -Force | Out-Null
}

$reportPath = Join-Path $testResultsDir $ReportName

# 3. Ejecutar pruebas con dotnet test
Write-Host "`n>> Ejecutando pruebas E2E..." -ForegroundColor Green
$sw = [System.Diagnostics.Stopwatch]::StartNew()

$csprojPath = Join-Path $projectRoot "Playwrigt_Demo.csproj"

dotnet test $csprojPath `
    --filter "$Filter" `
    --logger "html;LogFileName=$ReportName"

$exitCode = $LASTEXITCODE
$sw.Stop()

Write-Host "`n--------------------------------------------------------------------------" -ForegroundColor DarkGray
$colorFinal = "Yellow"
if ($exitCode -eq 0) { $colorFinal = "Green" }
Write-Host ">> Finalizado en $($sw.Elapsed.ToString('mm\:ss')) con codigo de salida: $exitCode" -ForegroundColor $colorFinal

# 4. Resumen de artefactos generados
Write-Host ""
Write-Host "========================== RESUMEN DE EVIDENCIAS ==========================" -ForegroundColor Cyan

if (Test-Path $reportPath) {
    Write-Host " [OK] Reporte HTML : $reportPath" -ForegroundColor Green
    if (-not $NoOpenReport) {
        Write-Host " [*] Abriendo reporte en el navegador predeterminado..." -ForegroundColor Yellow
        Start-Process $reportPath
    }
} else {
    Write-Host " [!] No se encontro el archivo de reporte en: $reportPath" -ForegroundColor Red
}

$videosSmokeDir = Join-Path $projectRoot "Reportes\Videos\Smoke"
if (Test-Path $videosSmokeDir) {
    $videoFiles = Get-ChildItem -Path $videosSmokeDir -Filter "*.webm" -ErrorAction SilentlyContinue
    $count = 0
    if ($videoFiles) { $count = $videoFiles.Count }
    Write-Host " [OK] Videos Smoke : $videosSmokeDir ($count videos grabados)" -ForegroundColor Green
} else {
    Write-Host " [i] Videos Smoke : $videosSmokeDir" -ForegroundColor Gray
}

$logsSmokeDir = Join-Path $projectRoot "Reportes\Logs\Smoke"
if (Test-Path $logsSmokeDir) {
    Write-Host " [OK] Logs de red  : $logsSmokeDir" -ForegroundColor Green
}

Write-Host "==========================================================================" -ForegroundColor Cyan
Write-Host ""

exit $exitCode
