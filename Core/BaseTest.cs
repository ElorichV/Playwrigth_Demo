using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

// --- AQUI VA EL TURBO ---
[assembly: Parallelizable(ParallelScope.Fixtures)] 
[assembly: LevelOfParallelism(2)] 
// ------------------------

namespace Playwrigt_Demo;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void LimpiarDirectorioDeVideos()
    {
        string rutaVideos = "../../../Reportes/Videos/";
        if (Directory.Exists(rutaVideos))
        {
            Directory.Delete(rutaVideos, true);
        }
        Directory.CreateDirectory(rutaVideos);
    }
}

public class BaseTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new() { RecordVideoDir = "../../../Reportes/Videos/" };

    [SetUp]
    public async Task SetupGlobal()
    {
        await Context.ClearCookiesAsync();
        await Task.Delay(1000);
    }

    [TearDown]
    public async Task NombrarVideoAlTerminar()
    {
        string nombreTest = TestContext.CurrentContext.Test.Name;
        foreach (var caracterInvalido in Path.GetInvalidFileNameChars())
        {
            nombreTest = nombreTest.Replace(caracterInvalido, '_');
        }
        
        await Context.CloseAsync();

        if (Page.Video != null)
        {
            string rutaFinal = Path.Combine("../../../Reportes/Videos/", $"{nombreTest}.webm");
            await Page.Video.SaveAsAsync(rutaFinal);
            await Page.Video.DeleteAsync();
        }
    }

    protected async Task LoginDinamico(string usuario, string password)
    {
        await Page.GotoAsync("https://capacitacionpinbox.seccionamarilla.com/", new() { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
        
        await Task.Delay(1000); 
        
        // FIX: Le damos 60 segundos completos de paciencia explícita a este primer elemento
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(usuario, new() { Timeout = 60000 });
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
    }
}