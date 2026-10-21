using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Text.Json; 
using System.Threading.Tasks;
using Playwrigt_Demo.Models; 

// ---------------------------------------------------------
// CONFIGURACIÓN DE RENDIMIENTO (HARDWARE)
// ---------------------------------------------------------
[assembly: Parallelizable(ParallelScope.Fixtures)] 
[assembly: LevelOfParallelism(1)] 

namespace Playwrigt_Demo;

/// <summary>
/// Motor principal de configuración global.
/// Se ejecuta UNA SOLA VEZ antes de que comience toda la suite de pruebas.
/// </summary>
[SetUpFixture]
public class GlobalSetup
{
    /// <summary>
    /// Gestiona la limpieza inteligente de evidencias. Solo elimina registros previos
    /// si se detecta explícitamente la bandera de ejecución completa de la suite.
    /// </summary>
    [OneTimeSetUp]
    public void LimpiarDirectoriosDeEvidencia()
    {
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        string[] directoriosReportes = { 
            Path.Combine(baseDir, "Reportes", "Videos"), 
            Path.Combine(baseDir, "Reportes", "Traces"),
            Path.Combine(baseDir, "Reportes", "Network"),
            Path.Combine(baseDir, "Reportes", "Logs")
        };

        // 🚨 CONTROL DE LIMPIEZA INTELIGENTE:
        // Solo eliminamos la evidencia vieja si se activa el flag CLEAR_ALL="1" desde la consola.
        if (Environment.GetEnvironmentVariable("CLEAR_ALL") == "1")
        {
            Console.WriteLine("[GLOBAL SETUP] Ejecución total detectada. Limpiando todo el historial de evidencias...");
            foreach (var directorio in directoriosReportes)
            {
                if (Directory.Exists(directorio))
                {
                    try { Directory.Delete(directorio, true); } catch { } 
                }
                Directory.CreateDirectory(directorio);
            }
        }
        else
        {
            Console.WriteLine("[GLOBAL SETUP] Ejecución modular/parcial detectada. Preservando el historial de otros módulos...");
            // Aseguramos que existan las carpetas raíz por si es la primera corrida en limpio
            foreach (var directorio in directoriosReportes)
            {
                if (!Directory.Exists(directorio)) Directory.CreateDirectory(directorio);
            }
        }
    }
}

/// <summary>
/// Clase base de la que DEBEN heredar todas las clases de pruebas (Tests) del framework.
/// </summary>
/// <remarks>
/// <para><b>Qué hace por cada prueba (ver <see cref="SetupGlobal"/>):</b> limpia cookies/storage,
/// navega a la URL del ambiente, arranca Tracing y registra los 3 manejadores globales de
/// SweetAlert (<see cref="AlertaDashboardApiLenta"/>, <see cref="AlertaSinInformacion"/>,
/// <see cref="AlertaCotizadorVacio"/>). Al terminar (<see cref="RecolectarEvidenciasAlTerminar"/>)
/// guarda video, trace y log, organizados por módulo bajo <c>Reportes/</c>. El tráfico de red
/// (HAR) se graba desde <see cref="ContextOptions"/> porque Playwright solo lo puede configurar
/// al crear el contexto, no en el <c>[SetUp]</c>.</para>
///
/// <para><b>Bug conocido del ambiente (no depende del framework de pruebas):</b> el dashboard del
/// agente dispara 6 llamadas (<c>/Agente/getDashboardCarteraInicial</c>, <c>getDashboardT</c>,
/// <c>diasVenta</c>, <c>getResumenPerformance</c>, <c>getResumenPubDigMensualSup</c>,
/// <c>getDashCategoriaAgente</c>) con <c>agente_id=0</c> en vez del ID real de sesión, y el
/// backend responde 500 en el 100% de las sesiones observadas. Eso dispara las alertas que
/// <see cref="AlertaDashboardApiLenta"/> y <see cref="AlertaSinInformacion"/> manejan. Ver
/// <c>docs/PROBLEMAS_CONOCIDOS.md</c> para el detalle y el estado de la corrección. Mientras
/// ese bug siga presente, es normal ver pruebas fallando por un SweetAlert interceptando un
/// clic — no es necesariamente un bug de la prueba.</para>
///
/// <para><b>Opciones de solución para ese bug (detalle completo en
/// <c>docs/PROBLEMAS_CONOCIDOS.md</c>, hallazgo 1):</b> (1) fix de raíz en backend/frontend para
/// que esas 6 llamadas envíen el <c>agente_id</c> real en vez de <c>0</c>; (2) mientras no se
/// corrija, correr la suite en lotes más pequeños (por <c>Category</c>/<c>Name</c>, ver README
/// sección 5) en vez de una corrida completa de un tirón, ya que la carga sostenida parece
/// agravar la frecuencia del bug; (3) <b>NO</b> agregar esperas o bloqueos manuales de modal
/// dentro de <see cref="LoginDinamico"/>/<see cref="LoginEspecifico"/> — ya causó una ronda de
/// fallos masivos por timeout en el pasado y es el anti-patrón que este proyecto decidió no
/// repetir.</para>
/// </remarks>
public class BaseTest : PageTest
{
    protected bool ModoAuditoriaRed = false;
    // 🚨 Locators de las 3 alertas conocidas de "API lenta de terceros" (Sección Amarilla).
    // Expuestos como propiedades para que pruebas específicas puedan desactivar SOLO la que
    // necesitan inspeccionar (con Page.RemoveLocatorHandlerAsync) sin apagar las otras dos.
    protected ILocator AlertaDashboardApiLenta => Page.Locator(".swal2-container").Filter(new() { HasText = "seguir navegando en Pinbox" });
    protected ILocator AlertaSinInformacion => Page.Locator(".swal2-container").Filter(new() { HasText = "no contiene información" });
    protected ILocator AlertaCotizadorVacio => Page.Locator(".swal2-container").Filter(new() { HasText = "No se encontraron resultados" });
    
    private const int TIMEOUT_LIMITE = 45000;
    protected ConfigData Config;

    /// <summary>
    /// Identifica el módulo correspondiente inspeccionando el prefijo del nombre de la prueba.
    /// </summary>
    private string ExtraerModulo(string nombreTest)
    {
        if (string.IsNullOrEmpty(nombreTest)) return "Generales";
        string upper = nombreTest.ToUpper();
        if (upper.Contains("LGN")) return "Login";
        if (upper.Contains("SMK")) return "Smoke";
        if (upper.Contains("PRN")) return "PantallasPrincipales";
        if (upper.Contains("CLN")) return "Clientes";
        if (upper.Contains("CTR")) return "Contratos";
        if (upper.Contains("GST") || upper.Contains("COT")) return "Cotizaciones";
        // Barredor de pantallas de Gestión mapeadas con codegen (ver docs/codegen_pantallas_gestion.txt):
        // Visor de Plantillas, Documentos, Escáner ADN+, Buscador Cliente, Cambios Digitales,
        // Oportunidades, Envío de Material, Asignación, Cartera Vencida, Movimientos de Campaña,
        // Casos, Sinergia Histórico, Estado de Cuenta Sinergia, Clientes Cercanos, Contratos
        // Guardados, Entrevista, Estadísticas de Citas, Estatus de Productos, Prospectos,
        // Prospectos TMX, Referencias de Contratos, Reporte de Venta, Solicitud de Propuestas,
        // Vista Panorámica, Prospectos Calificados, Creación HTBS.
        if (upper.Contains("VDP") || upper.Contains("DOC") || upper.Contains("ADN") || upper.Contains("BUS") ||
            upper.Contains("CAD") || upper.Contains("OPO") || upper.Contains("ENV") || upper.Contains("ASG") ||
            upper.Contains("CTV") || upper.Contains("MDC") || upper.Contains("CAS") || upper.Contains("SGH") ||
            upper.Contains("ECS") || upper.Contains("CLC") || upper.Contains("CTG") || upper.Contains("ENT") ||
            upper.Contains("ESC") || upper.Contains("EDP") || upper.Contains("PRS") || upper.Contains("PTX") ||
            upper.Contains("REF") || upper.Contains("RVT") || upper.Contains("SOP") || upper.Contains("VSP") ||
            upper.Contains("PRQ") || upper.Contains("HTB"))
            return "Gestion";
        return "Generales";
    }

    /// <summary>
    /// Convierte el nombre de una prueba (que puede traer espacios, paréntesis o comas de un
    /// <c>TestCaseData.SetName</c>) en un nombre de archivo válido para Windows.
    /// </summary>
    private string LimpiarNombreTest(string nombreOriginal)
    {
        string limpio = nombreOriginal.Replace(" ", "_").Replace("\"", "").Replace("(", "").Replace(")", "").Replace(",", "");
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            limpio = limpio.Replace(c.ToString(), "");
        }
        return limpio;
    }

    /// <summary>
    /// Configura el entorno de Playwright segmentando dinámicamente las salidas en subcarpetas por módulo.
    /// </summary>
    public override BrowserNewContextOptions ContextOptions() 
    {
        string nombreActual = TestContext.CurrentContext.Test.Name ?? "Desconocido";
        string nombreTest = LimpiarNombreTest(nombreActual);
        string modulo = ExtraerModulo(nombreTest);
        
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        // Obligamos a que la carpeta Network exista antes de grabar el HAR
        string dirNetwork = Path.Combine(baseDir, "Reportes", "Network", modulo);
        if (!Directory.Exists(dirNetwork)) Directory.CreateDirectory(dirNetwork);

        return new() 
        { 
            // 🚨 El video nace en una carpeta temporal segura para que Windows no lo bloquee
            RecordVideoDir = Path.Combine(baseDir, "Reportes", "Videos"),
            RecordHarPath = Path.Combine(dirNetwork, $"{nombreTest}_trafico.har"),
            RecordHarOmitContent = false, 
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 } 
        };
    }

    /// <summary>
    /// Se ejecuta antes de cada prueba. Deja la página limpia y lista en la pantalla de login
    /// (sesión y storage vacíos), arranca la captura de evidencia (Tracing) y registra los
    /// manejadores globales de SweetAlert. Ninguna clase de prueba debería tener que repetir
    /// esta configuración manualmente.
    /// </summary>
    [SetUp]
    public async Task SetupGlobal()
    {
        LogWriter($"--- INICIANDO PRUEBA: {TestContext.CurrentContext.Test.Name} ---");

        string rutaConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Global_Credentials.json");
        string jsonText = File.ReadAllText(rutaConfig);
        Config = JsonSerializer.Deserialize<ConfigData>(jsonText)!;

        Context.SetDefaultTimeout(TIMEOUT_LIMITE);
        Context.SetDefaultNavigationTimeout(TIMEOUT_LIMITE);
        Assertions.SetDefaultExpectTimeout(TIMEOUT_LIMITE);

        await Context.ClearCookiesAsync();
        await Page.GotoAsync(Config.Url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Page.EvaluateAsync("() => { localStorage.clear(); sessionStorage.clear(); }");
        await Page.ReloadAsync();

        await Context.Tracing.StartAsync(new()
        {
            Title = TestContext.CurrentContext.Test.Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true 
        });

        await Page.AddLocatorHandlerAsync(
            AlertaDashboardApiLenta,
            async () => {
                LogWriter("[ALERTA] API Lenta. Dando clic real en Continuar navegando...");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Continuar navegando." }).ClickAsync();
            }
        );

        await Page.AddLocatorHandlerAsync(
            AlertaSinInformacion,
            async () => {
                LogWriter("[ALERTA] Cliente sin información. Dando clic real en Aceptar...");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Aceptar" }).ClickAsync();
            }
        );
        // 🚨 Guardaespaldas para el popup del Cotizador
        await Page.AddLocatorHandlerAsync(
            AlertaCotizadorVacio,
            async () => {
                LogWriter("[ALERTA] Tabla de Cotizador vacía. Dando clic real en OK...");
                var btnOk = Page.GetByRole(AriaRole.Button, new() { Name = "OK" });
                await btnOk.ClickAsync();
                await Page.Locator(".swal2-container").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            }
        );
    }

    /// <summary>
    /// Se ejecuta después de cada prueba (pase o falle). Detiene el Tracing, cierra el contexto
    /// (lo que finaliza la grabación del video) y mueve video/trace desde su carpeta temporal a
    /// <c>Reportes/{Videos|Traces}/{Modulo}/{NombreDePrueba}</c>. Cualquier fallo al mover un
    /// archivo se registra pero nunca hace fallar la prueba — la evidencia es un extra, no una
    /// aserción.
    /// </summary>
    [TearDown]
    public async Task RecolectarEvidenciasAlTerminar()
    {
        string nombreTest = LimpiarNombreTest(TestContext.CurrentContext.Test.Name);
        string modulo = ExtraerModulo(nombreTest);
        
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        // 1. Aseguramos que la carpeta DESTINO FINAL del módulo exista
        string dirVideoModulo = Path.Combine(baseDir, "Reportes", "Videos", modulo);
        string dirTraceModulo = Path.Combine(baseDir, "Reportes", "Traces", modulo);
        if (!Directory.Exists(dirVideoModulo)) Directory.CreateDirectory(dirVideoModulo);
        if (!Directory.Exists(dirTraceModulo)) Directory.CreateDirectory(dirTraceModulo);

        // 2. Guardamos Trace directamente en su módulo
        try 
        {
            string rutaTrace = Path.Combine(dirTraceModulo, $"{nombreTest}_trace.zip");
            await Context.Tracing.StopAsync(new() { Path = rutaTrace });
        } 
        catch (Exception ex) { LogWriter($"[EVIDENCIA FALLIDA] Trace: {ex.Message}"); }

        // 3. Capturamos la ruta del video temporal ANTES de cerrar el contexto
        string rutaVideoOriginal = string.Empty;
        try { if (Page?.Video != null) rutaVideoOriginal = await Page.Video.PathAsync(); } catch { }

        // 4. Cerramos el navegador (esto le dice a Playwright que deje de grabar)
        try { await Context.CloseAsync(); } catch { }
        
        // 🚨 FRENO DE WINDOWS: Le damos medio segundo al Sistema Operativo para que suelte el archivo
        await Task.Delay(500); 
        
        // 5. Movemos el video desde la carpeta raíz temporal hasta la carpeta del Módulo
        try 
        {
            if (!string.IsNullOrEmpty(rutaVideoOriginal) && File.Exists(rutaVideoOriginal))
            {
                string rutaVideoDeseada = Path.Combine(dirVideoModulo, $"{nombreTest}.webm");
                if (File.Exists(rutaVideoDeseada)) File.Delete(rutaVideoDeseada);
                
                File.Move(rutaVideoOriginal, rutaVideoDeseada);
                LogWriter($"[EVIDENCIA] Video organizado exitosamente en: {modulo} / {nombreTest}.webm");
            }
        } 
        catch (Exception ex) { LogWriter($"[ERROR ARCHIVO] No se pudo mover el video: {ex.Message}"); }

        LogWriter($"--- FINALIZADO: {TestContext.CurrentContext.Test.Name} | ESTADO: {TestContext.CurrentContext.Result.Outcome.Status} ---\n");
    }

    /// <summary>
    /// Inicia sesión con la cuenta genérica de <c>TestData/Global_Credentials.json</c>. Úsalo en
    /// el <c>[SetUp]</c> de cualquier clase de prueba que no necesite un agente/cliente específico
    /// (la mayoría de los módulos de Ayudas, Dashboard y navegación). No lo llames una segunda vez
    /// dentro del mismo test ya logueado: eso reintroduciría un bug de "doble login" ya resuelto
    /// una vez (la sesión queda abierta y el campo "Usuario" ya no existe en pantalla).
    /// </summary>
    protected async Task LoginDinamico()
    {
        LogWriter("Inyectando credenciales dinámicas...");
    
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(Config.User);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(Config.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 15000 });
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        
        // Nota: el cierre de SweetAlerts inesperados (Dashboard, Sinergias, Cotizador, etc.)
        // ya lo gestionan los AddLocatorHandlerAsync registrados en SetupGlobal. Verificarlos
        // de nuevo aquí "a mano" con .swal2-confirm es peligroso: en modales de 2 botones
        // (ej. "Recargar" / "Continuar navegando.") esa clase no siempre apunta al botón que
        // queremos, y choca con el handler global, dejando la prueba esperando un botón que
        // ya no existe. Verificamos directamente el destino final del login.
        var menuOpenButton = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        await Expect(menuOpenButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 30000 });
        LogWriter("Login exitoso y Dashboard verificado.");
    }

    /// <summary>
    /// Inicia sesión con credenciales de un agente puntual (normalmente obtenidas de
    /// <c>ClientePoolFactory</c>). Úsalo cuando la prueba necesita que la sesión sea de un
    /// agente/cliente específico — p. ej. Cotizaciones y Alta de Clientes, donde cada caso de
    /// prueba se reparte entre varias cuentas reales para no saturar siempre la misma. No lleva
    /// un <c>[SetUp]</c> de login genérico en esas clases a propósito: si el login corre dos
    /// veces (una vez genérico en <c>[SetUp]</c> y otra aquí), la segunda encuentra la sesión ya
    /// abierta y el campo "Usuario" ya no existe en pantalla → timeout de 45s garantizado.
    /// </summary>
    protected async Task LoginEspecifico(string usuario, string password)
    {
        LogWriter($"Inyectando credenciales específicas para el agente: {usuario}");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(usuario);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Misma verificación final que LoginDinamico: sin esto, el test seguía adelante
        // sobre un dashboard que aún no terminaba de cargar, y el siguiente clic (p. ej.
        // el link "Cotizador") nunca encontraba su elemento → timeout de 45s en cascada.
        var menuOpenButton = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        await Expect(menuOpenButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 30000 });
    }
    
    /// <summary>
    /// Imprime en consola (para el reporte HTML) y guarda físicamente en un archivo .log
    /// </summary>
    protected void LogWriter(string mensaje)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string lineaLog = $"[{timestamp}] {mensaje}";
        
        // 1. Imprimir en consola normal (NUnit lo atrapa para el HTML)
        Console.WriteLine(lineaLog);

        // 2. Escribir en un archivo .log físico
        try 
        {
            // Obtener el nombre de la prueba y la ruta base (afuera de bin)
            string nombreActual = TestContext.CurrentContext.Test.Name ?? "Ejecucion_Global";
            string nombreTest = LimpiarNombreTest(nombreActual);
            string modulo = ExtraerModulo(nombreTest);
            string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            
            // Crear la carpeta del módulo dentro de Logs (ej. Reportes/Logs/Smoke)
            string dirLogs = Path.Combine(baseDir, "Reportes", "Logs", modulo);
            if (!Directory.Exists(dirLogs)) Directory.CreateDirectory(dirLogs);

            // Anexar la línea al archivo (AppendAllText crea el archivo si no existe)
            string rutaArchivoLog = Path.Combine(dirLogs, $"{nombreTest}.log");
            File.AppendAllText(rutaArchivoLog, lineaLog + Environment.NewLine);
        }
        catch 
        { 
            // Ignorar errores de I/O silenciosamente para no hacer fallar la prueba
        }
    }

    /// <summary>
    /// Clic "seguro": espera a que el elemento esté visible (hasta <see cref="TIMEOUT_LIMITE"/>),
    /// hace clic, y si el clic tarda más de 15s registra una advertencia de latencia en el log.
    /// Es el reemplazo estándar de <c>ILocator.ClickAsync()</c> en este proyecto — úsalo en vez
    /// del clic directo salvo que tengas una razón concreta para no medir latencia.
    /// </summary>
    protected async Task ClickConMonitoreo(ILocator localizador, string nombreAccion)
    {
        // Esperamos a que el elemento sea visible y estable antes de hacer clic. Usamos el mismo
        // límite que el resto del framework (45s): con 10s fijos, un SweetAlert cruzándose en
        // medio del clic (el handler tarda unos segundos en cerrarlo) ya alcanzaba a agotar
        // el timeout antes de que el elemento quedara clickeable.
        await localizador.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = TIMEOUT_LIMITE });

        var cronometro = System.Diagnostics.Stopwatch.StartNew();
        await localizador.ClickAsync();
        cronometro.Stop();
    
        if (cronometro.ElapsedMilliseconds > 15000) 
            LogWriter($"[⚠️ ADVERTENCIA] Latencia detectada en {nombreAccion}: {cronometro.ElapsedMilliseconds}ms.");
    }
}