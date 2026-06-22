using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTERFAZ GRÁFICA DEL MENÚ (UI)
// ---------------------------------------------------------
/// <summary>
/// Pruebas atómicas enfocadas en los micro-componentes de la UI. 
/// Válida colapsos, animaciones y limpieza del DOM.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
[Category("Media")]
public class QA_SMK_MenuLateral_UI_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuUI()
    {
        await LoginDinamico();
        LogWriter("Configurando punto de partida en Pestaña Dashboard.");
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }

    [Test]
    public async Task QA_SMK_01_AperturaCierreHamburguesa()
    {
        LogWriter("Iniciando Prueba atómica: Apertura y Cierre del Menú de Navegación.");
        var botonMenu = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });

        // ABRIR
        await ClickConMonitoreo(botonMenu, "Apertura de Menú Lateral");
        await Expect(Page.Locator("a:has-text('Salir')").First).ToBeVisibleAsync();

        // CERRAR (Validación de colapso del DOM)
        var botonCerrar = Page.GetByRole(AriaRole.Button, new() { Name = "Close Menu" });
        await ClickConMonitoreo(botonCerrar, "Cierre de Menú Lateral");
        await Expect(Page.Locator("a:has-text('Salir')").First).ToBeHiddenAsync(new() { Timeout = 3000 });
        
        LogWriter("Animaciones y estados del DOM del menú principal validados.");
    }
}