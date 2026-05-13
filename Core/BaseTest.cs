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
// ⚠️ NOTA: El nivel de paralelismo está estrictamente en 1 para 
// evitar estrangulamiento de red y CPU en ejecuciones locales.
[assembly: Parallelizable(ParallelScope.Fixtures)] 
[assembly: LevelOfParallelism(1)] 

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// PREPARACIÓN GLOBAL DEL FRAMEWORK
// ---------------------------------------------------------
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void LimpiarDirectoriosDeEvidencia()
    {
        // Limpiamos las carpetas de reportes antes de iniciar la suite
        string[] directoriosReportes = { 
            "../../../Reportes/Videos/", 
            "../../../Reportes/Traces/",
            "../../../Reportes/Network/",
            "../../../Reportes/Logs/"
        };

        foreach (var directorio in directoriosReportes)
        {
            if (Directory.Exists(directorio))
            {
                Directory.Delete(directorio, true);
            }
            Directory.CreateDirectory(directorio);
        }
    }
}

// ---------------------------------------------------------
// MOTOR PRINCIPAL DE PRUEBAS (BaseTest)
// ---------------------------------------------------------
public class BaseTest : PageTest
{
    protected bool ModoAuditoriaRed = false;
    // Límite de paciencia para las APIs de Pinbox
    private const int TIMEOUT_LIMITE = 30000;
    protected ConfigData Config;

    // ---------------------------------------------------------
    // CONFIGURACIÓN DE CONTEXTO NATIVO (VIDEO Y RED)
    // ---------------------------------------------------------
    public override BrowserNewContextOptions ContextOptions() 
    {
        // Generamos un nombre seguro para nombrar los archivos de red dinámicamente
        string nombreTest = TestContext.CurrentContext.Test.Name.Replace(" ", "_").Replace("\"", "");

        return new() 
        { 
            // 🎬 Grabación de Video Automática
            RecordVideoDir = "../../../Reportes/Videos/",
            
            // 📡 Grabación de Tráfico de Red (.har) Dinámica por cada Test
            RecordHarPath = $"../../../Reportes/Network/{nombreTest}_trafico.har",
            RecordHarOmitContent = false, // Guarda el JSON y respuestas de las APIs para Sistemas
            
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        };
    }

    // ---------------------------------------------------------
    // SETUP POR PRUEBA
    // ---------------------------------------------------------
    [SetUp]
    public async Task SetupGlobal()
    {
        Console.WriteLine($"--- INICIANDO PRUEBA: {TestContext.CurrentContext.Test.Name} ---");

        // 1. CARGA DE CONFIGURACIÓN GLOBAL
        string jsonText = File.ReadAllText("../../../TestData/Global_Credentials.json");
        Config = JsonSerializer.Deserialize<ConfigData>(jsonText)!;

        // 2. TOLERANCIA DINÁMICA
        Context.SetDefaultTimeout(TIMEOUT_LIMITE);
        Context.SetDefaultNavigationTimeout(TIMEOUT_LIMITE);
        Assertions.SetDefaultExpectTimeout(TIMEOUT_LIMITE);

        // 3. HIGIENE DE SESIÓN
        await Context.ClearCookiesAsync();
        await Page.GotoAsync(Config.Url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Page.EvaluateAsync("() => { localStorage.clear(); sessionStorage.clear(); }");
        await Page.ReloadAsync();

        // 4. INICIO DE TRACING (Radiografía profunda de la UI)
        await Context.Tracing.StartAsync(new()
        {
            Title = TestContext.CurrentContext.Test.Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true 
        });

        // 5. CORTAFUEGOS GLOBALES CONTRA POPUPS
        await Page.AddLocatorHandlerAsync(
            Page.GetByText("Continuar navegando"),
            async () => {
                Console.WriteLine("[ALERTA] Popup 'Continuar navegando' detectado y gestionado.");
                await Page.Locator(".swal2-cancel").Filter(new() { HasText = "Continuar navegando" }).ClickAsync();
            }
        );

        await Page.AddLocatorHandlerAsync(
            Page.GetByText("La búsqueda ingresada no contiene información"),
            async () => {
                Console.WriteLine("[ALERTA] Popup 'Sin información' detectado. Aplicando clic forzado.");
                await Page.Locator(".swal2-cancel").Filter(new() { HasText = "Continuar navegando" }).ClickAsync(new() { Force = true });
            }
        );
    }

// ---------------------------------------------------------
    // TEARDOWN (Cierre seguro y Renombrado de Evidencias)
    // ---------------------------------------------------------
    [TearDown]
    public async Task RecolectarEvidenciasAlTerminar()
    {
        string nombreTest = TestContext.CurrentContext.Test.Name.Replace(" ", "_").Replace("\"", "");

        // 1. CIERRE DE TRACE
        string rutaTrace = Path.Combine("../../../Reportes/Traces/", $"{nombreTest}_trace.zip");
        await Context.Tracing.StopAsync(new() { Path = rutaTrace });

        // 2. OBTENER RUTA DEL VIDEO (Antes de que Playwright se apague)
        string rutaVideoOriginal = string.Empty;
        if (Page?.Video != null)
        {
            rutaVideoOriginal = await Page.Video.PathAsync();
        }

        // 3. CIERRE DE CONTEXTO (¡SÚPER IMPORTANTE!)
        // Libera la RAM de tu ThinkPad y suelta el archivo de video para que podamos modificarlo.
        await Context.CloseAsync();
        
        // 4. RENOMBRADO SEGURO DEL VIDEO (OS Level)
        if (!string.IsNullOrEmpty(rutaVideoOriginal) && File.Exists(rutaVideoOriginal))
        {
            string rutaVideoDeseada = Path.Combine("../../../Reportes/Videos/", $"{nombreTest}.webm");
            
            // Si existe un video de una corrida anterior, lo sobreescribimos
            if (File.Exists(rutaVideoDeseada)) File.Delete(rutaVideoDeseada);
            
            File.Move(rutaVideoOriginal, rutaVideoDeseada);
            Console.WriteLine($"[EVIDENCIA] Video renombrado: {rutaVideoDeseada}");
        }

        Console.WriteLine($"--- FINALIZADO: {TestContext.CurrentContext.Test.Name} | ESTADO: {TestContext.CurrentContext.Result.Outcome.Status} ---\n");
    }

    // ---------------------------------------------------------
    // MÉTODOS DE UTILERÍA
    // ---------------------------------------------------------
    protected async Task LoginDinamico()
    {
        Console.WriteLine("Inyectando credenciales dinámicas...");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(Config.User);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(Config.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
        
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        Console.WriteLine("Login exitoso.");
    }
    // ---------------------------------------------------------
    // MÉTODOS DE UTILERÍA (Versión Ligera - Consola)
    // ---------------------------------------------------------
    protected void LogWriter(string mensaje)
    {
        // Ahora solo imprime en la consola de Rider, sin saturar el disco
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[{timestamp}] {mensaje}");
    }

    protected async Task ClickConMonitoreo(ILocator localizador, string nombreAccion)
    {
        // Monitor de rendimiento seguro en memoria
        var cronometro = System.Diagnostics.Stopwatch.StartNew();
        
        await localizador.ClickAsync();
        
        cronometro.Stop();
        if (cronometro.ElapsedMilliseconds > 15000) // 15 segundos
        {
            Console.WriteLine($"[⚠️ ADVERTENCIA] Latencia detectada en {nombreAccion}: {cronometro.ElapsedMilliseconds}ms. Reportar a sistemas.");
        }
    }

}