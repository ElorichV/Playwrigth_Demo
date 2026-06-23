# Framework de Automatización E2E - Pinbox 🚀

Este repositorio contiene la suite de pruebas automatizadas End-to-End (E2E) para la plataforma **Pinbox** (Sección Amarilla). El framework está desarrollado en **C#** utilizando **Playwright** como motor de automatización, **NUnit** como test runner, y está montado sobre **.NET 10.0**.

El objetivo principal de este proyecto es garantizar la estabilidad de los flujos críticos del sistema mediante pruebas de humo (Smoke Tests) y flujos operativos complejos (Dashboard, Gestión, Clientes y Cotizaciones), asegurando tolerancia a la latencia de red y manejo de comportamiento asíncrono de la interfaz de usuario.

---

## 🏗️ Características Principales de la Arquitectura

### 1. BaseTest Centralizado e Inteligente
Toda la suite de pruebas hereda de una clase base común `BaseTest : PageTest` que automatiza y homogeneiza el ciclo de vida de las pruebas:
* **Aislamiento de Estado**: Limpieza automática de cookies, `localStorage` y `sessionStorage` antes de iniciar cada test para evitar contaminación de datos entre ejecuciones.
* **Timeouts Unificados**: Configuración estricta de límites de tiempo de navegación, de contexto y aserciones mediante constantes compartidas (`TIMEOUT_LIMIT`).
* **Trazabilidad Avanzada**: Generación automática de reportes interactivos, capturas de pantalla (*Screenshots*), árboles de elementos (*Snapshots*) y código fuente (*Sources*) para fallas mediante Playwright Tracing.

### 2. Intercepción Dinámica de Alertas (Cortafuegos de UI)
Para mitigar la intermitencia provocada por llamadas lentas de la API o modales asíncronos sorpresivos generados por **SweetAlert2**, el framework implementa un vigilante pasivo mediante el uso de `Page.AddLocatorHandlerAsync()`.
Este mecanismo intercepta y despacha automáticamente en milisegundos las siguientes ventanas emergentes sin romper el hilo de ejecución principal de las pruebas:
* **Modal de Inactividad / API Lenta**: Detecta el texto *"seguir navegando en Pinbox"* y ejecuta un clic nativo confiable en *"Continuar navegando."*, esperando a que la animación CSS concluya.
* **Modal de Filtros Vacíos**: Intercepta avisos de *"no contiene información"* dentro del Dashboard, haciendo clic en *"Aceptar"* y controlando el estado de desvanecimiento del elemento en el DOM.
* **Modal del Cotizador**: Monitorea avisos de *"No se encontraron resultados"* para usuarios nuevos sin histórico, presionando el botón *"OK"* automáticamente para liberar la UI.

### 3. Pruebas Guiadas por Datos (Data-Driven Testing con JSON)
Separación absoluta entre la lógica de los scripts y la data operativa. La información requerida para los flujos vive dentro del directorio `TestData/` en archivos estructurados en formato **JSON** (Credenciales, Clientes Fijos, KPIs a validar). El framework deserializa dinámicamente esta información durante la inicialización global (`SetupGlobal`).

### 4. Robustez contra Animaciones y Latencia
Se evita el uso de esperas explícitas genéricas (*flaky waits*). En su lugar, el framework aprovecha el auto-waiting de Playwright combinado con aserciones especializadas de Viewport (`Not.ToBeInViewportAsync`) para lidiar con menús laterales deslizantes y loaders dinámicos (`#divLoading`).

---

## 📂 Estructura General del Proyecto

```text
Playwrigt_Demo/
│
├── Tests/                               # Clases de pruebas organizadas por módulos de negocio
│   ├── QA_SMK_MenuLateral_UI_Tests.cs    # Pruebas de Humo: Menú hamburguesa, colapsos y UI base
│   ├── QA_PRN_IntegridadDashboardTests.cs# Validación de KPIs (OLBC, Contratos), filtros de Sinergia y Tabs
│   ├── QA_CLN_GestionClientesTests.cs    # Flujos de Alta Exitosa (Física/Moral) y validación de campos obligatorios
│   └── QA_GST_CotizacionTests.cs         # Suite de pruebas para el módulo de Cotizaciones e histórico
│
├── TestData/                            # Datos dinámicos de pruebas e identidades (Excluidos de Git)
│   ├── Global_Credentials.example.json  # Plantilla base para credenciales del sistema
│   ├── QA_LGN_Data.example.json         # Plantilla base de usuarios para flujos de Login e IAM
│   └── CatalogoMaestro.json             # Catálogo estático de links, IDs y selectores del sistema
│
├── BaseTest.cs                          # Configuración global, inicialización del navegador y LocatorHandlers
├── Playwrigt_Demo.csproj                # Archivo de definición del proyecto .NET
└── .gitignore                           # Exclusiones de Git optimizadas para entornos JetBrains/VS
```

---

## 🛠️ Requisitos Previos

Antes de configurar el proyecto, asegúrate de tener instalado lo siguiente en tu máquina local:
* [SDK de .NET 10.0](https://dotnet.microsoft.com/download)
* Un IDE compatible: [JetBrains Rider](https://www.jetbrains.com/rider/) (Recomendado) o [Visual Studio 2022](https://visualstudio.microsoft.com/)
* [Git](https://git-scm.com/) (Instalar con Git Bash en entornos Windows)

---

## ⚙️ Instalación y Configuración Local

Sigue estos pasos detallados para montar el entorno de pruebas en tu computadora personal o de trabajo:

### 1. Clonar el repositorio
Abre tu terminal de confianza (o Git Bash) y ejecuta:
```bash
git clone https://github.com/ElorichV/Playwrigth_Demo.git
cd Playwrigt_Demo
```

### 2. Configurar los Archivos de Datos (`TestData`)
Por motivos de seguridad, los archivos reales con contraseñas e identificadores específicos de clientes están excluidos del repositorio a través del `.gitignore`. 
Para habilitarlos de forma local:
1.  Ve al directorio `TestData/`.
2.  Duplica los archivos que terminen en `.example.json`.
3.  Renombra las copias eliminando el fragmento `.example` (de modo que queden como `Global_Credentials.json`, `QA_LGN_Data.json`, etc.).
4.  Abre los nuevos archivos JSON y coloca las credenciales válidas y los datos proporcionados por la matriz de negocio (cruces verificados en el sistema **IAM**).

### 3. Limpiar y Compilar la Solución
Desde tu IDE de preferencia (Rider o Visual Studio), realiza una acción de **Clean Solution** seguido de un **Rebuild Solution**.
Si prefieres hacerlo mediante la línea de comandos de .NET, ejecuta:
```bash
dotnet clean
dotnet build
```
*Este paso restaurará de forma automática todos los paquetes de NuGet declarados (Playwright, NUnit, Text.Json).*

### 4. Instalar los Binarios de los Navegadores
Playwright requiere sus propios binarios dedicados para ejecutar pruebas de forma aislada. Después de compilar la solución por primera vez, instala los navegadores ejecutando el script nativo generado desde tu terminal:

**En Windows (PowerShell):**
```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install
```
**En Git Bash / Linux / macOS:**
```bash
./bin/Debug/net10.0/playwright.ps1 install
```

---

## 🚀 Ejecución de Pruebas

Puedes disparar la suite completa de pruebas de diversas maneras:

### Desde el IDE (Rider / Visual Studio)
1. Abre la ventana del **Unit Tests Explorer** o **Test Explorer**.
2. Presiona el botón de **Run All** o selecciona una categoría/módulo en específico (por ejemplo, expandir y ejecutar solo `QA_CLN_GestionClientesTests`).

### Desde la Consola de Comandos (.NET CLI)
Para correr absolutamente toda la suite y generar el reporte por defecto:
```bash
dotnet test
```

---

## 🔒 Buenas Prácticas del Repositorio y Git

Para mantener el repositorio limpio y proteger información sensible de la empresa, el archivo `.gitignore` está configurado para omitir rigurosamente:
* Carpetas de compilación temporales y binarios (`bin/`, `obj/`).
* Configuraciones locales de los entornos de desarrollo (`.idea/`, `.vs/`, `*.DotSettings.user`).
* Resultados de ejecuciones anteriores (`test-results/`, `TestResults/`, `playwright-report/`).
* Archivos reales de datos que contienen tokens o contraseñas en claro (`Global_Credentials.json`, `QA_LGN_Data.json`, `ClienteFijo.json`, `QA_GST_ClientesAprobados.json`).

**⚠️ Nota Importante:** Al crear nuevos archivos JSON con datos sensibles dentro de la carpeta `TestData`, 
asegúrate siempre de que estén declarados explícitamente en el `.gitignore` antes de realizar un `git commit`. 
Siempre provee una plantilla `.example.json` con valores genéricos vacíos si cambias la estructura de los mismos.
