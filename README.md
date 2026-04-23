# 🎭 Playwright Demo - Proyecto de Pruebas Automatizadas

Proyecto de automatización de pruebas End-to-End (E2E) para la plataforma Pinbox utilizando Playwright con C# y NUnit.

## 📋 Tabla de Contenidos

- [Descripción](#-descripción)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Ejecución de Pruebas](#-ejecución-de-pruebas)
- [Configuración](#-configuración)
- [Casos de Prueba](#-casos-de-prueba)
- [Reportes](#-reportes)
- [Contribución](#-contribución)

## 🎯 Descripción

Framework de pruebas automatizadas escalable y mantenible para la plataforma Pinbox, implementando:

- **Arquitectura**: Page Object Model (POM) / Domain-Driven
- **Metodología**: Pruebas E2E de Caja Negra (Black-Box)
- **Estrategia**: Smoke Tests estructurales + Deep Dives funcionales
- **Paralelización**: Ejecución concurrente de pruebas para optimizar tiempos

## 🛠️ Tecnologías

- **Lenguaje**: C# (.NET 10.0)
- **Framework de Pruebas**: NUnit 3.14.0
- **Automatización UI**: Microsoft Playwright 1.58.0
- **Reportes**: HTML Test Logger
- **Gestión de Datos**: JSON (Data-Driven Testing)

## 📦 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) o superior
- [PowerShell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell) (para instalación de navegadores)
- IDE recomendado: [Visual Studio 2022](https://visualstudio.microsoft.com/) o [JetBrains Rider](https://www.jetbrains.com/rider/)

## 🚀 Instalación

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd Playwrigt_Demo
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Instalar navegadores de Playwright

**⚠️ PASO CRÍTICO**: Playwright requiere la instalación de navegadores específicos.

```powershell
# Windows PowerShell
pwsh bin/Debug/net10.0/playwright.ps1 install

# O si usas PowerShell Core
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

### 4. Compilar el proyecto

```bash
dotnet build
```

### 5. Verificar instalación

```bash
dotnet test --list-tests
```

## 📁 Estructura del Proyecto

```
Playwrigt_Demo/
├── Core/
│   └── BaseTest.cs              # Clase base con configuración global
├── Models/
│   ├── KpiTestData.cs           # Modelo de datos para KPIs
│   └── UsuarioTestData.cs       # Modelo de datos para usuarios
├── TestData/
│   ├── kpis.json                # Datos de prueba para KPIs
│   └── usuarios.json            # Datos de prueba para autenticación
├── Tests/
│   ├── Autenticacion/
│   │   └── LoginTests.cs        # TC01-TC03: Pruebas de login
│   ├── Dashboard/
│   │   ├── IntegridadDashboardTests.cs    # TC04-TC06: Validación de UI
│   │   ├── NavegacionKpisTests.cs         # TC07: Enlaces de KPIs
│   │   └── FiltrosInteraccionTests.cs     # TC08-TC10: Filtros y búsqueda
│   └── MenuLateral/
│       └── MenuLateralTests.cs  # TC11-TC21: Navegación de menús
├── Reportes/
│   └── Videos/                  # Grabaciones de ejecución de pruebas
├── TestResults/                 # Reportes HTML generados
├── Playwrigt_Demo.csproj        # Configuración del proyecto
├── Playwrigt_Demo.sln           # Solución de Visual Studio
├── TEST_PLAN.md                 # Plan de pruebas detallado
└── README.md                    # Este archivo
```

## ▶️ Ejecución de Pruebas

### Ejecutar todas las pruebas

```bash
dotnet test --logger "html;LogFileName=reporte_completo.html"
```

### Ejecutar pruebas por módulo

```bash
# Solo pruebas de autenticación
dotnet test --filter "FullyQualifiedName~LoginTests" --logger "html;LogFileName=reporte_login.html"

# Solo pruebas de dashboard
dotnet test --filter "FullyQualifiedName~DashboardTests" --logger "html;LogFileName=reporte_dashboard.html"

# Solo pruebas de menú lateral
dotnet test --filter "FullyQualifiedName~MenuLateralTests" --logger "html;LogFileName=reporte_menu.html"
```

### Ejecutar un caso de prueba específico

```bash
# Por nombre de método
dotnet test --filter "Name~TC04_ValidarIdentidadEnDashboard" --logger "html;LogFileName=reporte_tc04.html"

# Por caso específico del JSON (Data-Driven)
dotnet test --filter "Name~ValidarFlujoDeLogin(TC02)" --logger "html;LogFileName=reporte_tc02.html"
```

### Ejecución en paralelo

El proyecto está configurado para ejecutar pruebas en paralelo (2 hilos simultáneos). Para modificar:

```csharp
// En Core/BaseTest.cs
[assembly: LevelOfParallelism(4)] // Cambiar el número según tus recursos
```

## ⚙️ Configuración

### Datos de Prueba

Los datos de prueba se gestionan mediante archivos JSON en la carpeta `TestData/`:

**⚠️ IMPORTANTE - SEGURIDAD**: 
- Los archivos `usuarios.json` y `kpis.json` contienen **datos sensibles** (credenciales, URLs internas, nombres de usuarios)
- Estos archivos están **excluidos del repositorio** mediante `.gitignore`
- **NO subas estos archivos a GitHub bajo ninguna circunstancia**
- Para obtener los datos de prueba, contacta al equipo de QA o al administrador del proyecto

**Estructura esperada de los archivos**:
- `usuarios.json` - Credenciales para pruebas de autenticación (TC01-TC03)
- `kpis.json` - Datos para validación de KPIs del dashboard (TC07)

### Grabación de Videos

Las pruebas graban automáticamente videos de cada ejecución:

- **Ubicación**: `Reportes/Videos/`
- **Formato**: `.webm`
- **Nombre**: Basado en el nombre del caso de prueba

Para deshabilitar la grabación, modifica `BaseTest.cs`:

```csharp
public override BrowserNewContextOptions ContextOptions() => new() 
{ 
    // RecordVideoDir = "../../../Reportes/Videos/" // Comentar esta línea
};
```

### Timeouts

Los timeouts están configurados para entornos de capacitación/QA:

- **Navegación**: 60 segundos
- **Elementos UI**: 30 segundos (críticos), 15 segundos (estándar)

## 🧪 Casos de Prueba

### Módulo 1: Autenticación (TC01-TC03)
- ✅ Login exitoso con credenciales válidas
- ✅ Login denegado por contraseña incorrecta
- ✅ Login denegado por usuario inexistente

### Módulo 2: Dashboard (TC04-TC10)
- ✅ Validación de identidad del agente
- ✅ Carga de KPIs base
- ✅ Interactividad de tarjetas de estado
- ✅ Enlaces de navegación de KPIs
- ✅ Navegación por pestañas
- ✅ Filtros de sinergia
- ✅ Manejo de buscador vacío

### Módulo 3: Menú Lateral (TC11-TC21)
- 🔄 Apertura/cierre de menú hamburguesa
- 🔄 Validación de cambio de contexto
- 🔄 Redirecciones del menú operativo
- 🔄 Redirecciones del menú de gestión
- 🔄 Redirecciones del menú de ayuda

**Leyenda**: ✅ Completado | 🔄 En desarrollo | ❌ Pendiente

Para ver el plan completo de pruebas, consulta [TEST_PLAN.md](TEST_PLAN.md).

## 📊 Reportes

### Reportes HTML

Los reportes se generan automáticamente en la carpeta `TestResults/`:

```bash
# Abrir el último reporte generado
start TestResults/reporte_completo.html
```

### Videos de Ejecución

Cada prueba genera un video en `Reportes/Videos/` con el nombre del caso de prueba.

### Capturas de Pantalla

Las capturas de evidencia se almacenan en `Reportes/`:
- `Evidencia_TC01.png`
- `Evidencia_TC02.png`
- `Evidencia_TC03.png`

## 🤝 Contribución

### Agregar Nuevos Casos de Prueba

1. **Crear el archivo de prueba** en la carpeta correspondiente dentro de `Tests/`
2. **Heredar de BaseTest** para aprovechar la configuración global
3. **Usar Data-Driven Testing** cuando sea posible (JSON + TestCaseSource)
4. **Actualizar TEST_PLAN.md** con los nuevos casos

Ejemplo:

```csharp
[TestFixture]
public class NuevoModuloTests : BaseTest
{
    [Test]
    public async Task TC99_NuevoCasoDePrueba()
    {
        // Usar LoginDinamico con credenciales del JSON
        await LoginDinamico(usuario, password);
        
        // Tu lógica de prueba aquí
        
        await Expect(Page.Locator("#elemento")).ToBeVisibleAsync();
    }
}
```

### Buenas Prácticas

- ✅ Usar selectores semánticos (AriaRole) cuando sea posible
- ✅ Implementar esperas explícitas con timeouts razonables
- ✅ Limpiar cookies/storage entre pruebas
- ✅ Nombrar casos de prueba con formato `TC##_DescripcionClara`
- ✅ Documentar casos complejos con comentarios
- ✅ Mantener los datos de prueba en archivos JSON separados

## 📝 Notas Adicionales

### Comandos Útiles

```bash
# Generar código de prueba interactivamente
pwsh bin/Debug/net10.0/playwright.ps1 codegen <URL_DEL_SISTEMA>

# Limpiar artefactos de compilación
dotnet clean

# Reconstruir proyecto
dotnet build --no-incremental

# Ver versión de Playwright instalada
dotnet list package | findstr Playwright
```

### Solución de Problemas

**Error: "Executable doesn't exist"**
```bash
# Reinstalar navegadores de Playwright
pwsh bin/Debug/net10.0/playwright.ps1 install --force
```

**Error: "Timeout waiting for element"**
- Verificar que el entorno de capacitación esté disponible
- Aumentar los timeouts en casos específicos
- Revisar selectores con `codegen`

**Videos no se generan**
- Verificar que la carpeta `Reportes/Videos/` exista
- Revisar permisos de escritura en el directorio

## 📄 Licencia

Este proyecto es de uso interno para pruebas de la plataforma Pinbox.

## 👥 Autores

- **Equipo de QA** - Automatización de pruebas

---

**Última actualización**: 2025

Para más información, consulta el [Plan de Pruebas Detallado](TEST_PLAN.md).
