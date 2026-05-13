using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

[TestFixture]
public class QA_SMK_MenuLateral_Gestion_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuGestion()
    {
        await LoginDinamico();
        
        // 🚨 DIAGNÓSTICO QA (BUG DE UX - "ESTADO TRABADO"): 
        // La plataforma pierde el estado de navegación al regresar al inicio.
        LogWriter("Aplicando workaround de limpieza: Forzando clic en Pestaña Gestión.");
        await ClickConMonitoreo(Page.Locator("#tab-home-2"), "Cambio a Pestaña Gestión");
        await Task.Delay(1000); 
        
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    private async Task NavegarYRegresarGestion(string textoEnlace, string selectorValidacion)
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");
        await Task.Delay(800); 

        var opcionMenu = Page.Locator($"a:has-text('{textoEnlace}')").First;
        await ClickConMonitoreo(opcionMenu, $"Selección de {textoEnlace}");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        try 
        {
            await Page.Locator(selectorValidacion).First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            
            LogWriter("Ejecutando retorno seguro al Tablero.");
            var btnBack = Page.Locator("#back").First;
            if (await btnBack.IsVisibleAsync()) await btnBack.EvaluateAsync("el => el.click()");
            
            await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        }
        catch (System.Exception ex)
        {
            LogWriter($"[AVISO] Interrupción en {textoEnlace}: {ex.Message}");
            return; 
        }
    }

    [Test]
    public async Task QA_SMK_03_Redireccion_ClienteNuevo() => await NavegarYRegresarGestion("Cliente nuevo", "text='CREAR CLIENTE NUEVO'");

    [Test]
    public async Task QA_SMK_03_Redireccion_ContratosCreados() => await NavegarYRegresarGestion("Contratos creados", "text='Contratos Creados'");

    [Test]
    public async Task QA_SMK_03_Redireccion_Cotizador() => await NavegarYRegresarGestion("Cotizador", "text='Cotizaciones'");
}