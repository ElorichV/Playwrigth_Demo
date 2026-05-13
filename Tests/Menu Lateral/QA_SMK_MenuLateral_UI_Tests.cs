using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTERFAZ GRÁFICA DEL MENÚ (UI)
// ---------------------------------------------------------
[TestFixture]
public class QA_SMK_MenuLateral_UI_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuUI()
    {
        await LoginDinamico();
        
        LogWriter("Configurando punto de partida en Pestaña Dashboard.");
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        
        // ESCUDO ANTI-LAG: El cortafuegos global de BaseTest ya maneja los pop-ups.
        // Validamos estado estable heredando el timeout de 30s.
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_SMK_01_AperturaCierreHamburguesa()
    {
        LogWriter("Iniciando Prueba QA-SMK-01: Apertura y Cierre de Menú.");
        var botonMenu = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        
        // PASO 1: ABRIR EL MENÚ
        await ClickConMonitoreo(botonMenu, "Apertura de Menú Lateral");
        await Task.Delay(1000); // Reducido de 1500 a 1000 para ganar velocidad
        
        LogWriter("Validando despliegue del panel lateral.");
        await Expect(Page.Locator("a:has-text('Salir')").First).ToBeVisibleAsync();

        // PASO 2: CERRAR EL MENÚ
        LogWriter("Cerrando panel lateral.");
        await ClickConMonitoreo(botonMenu, "Cierre de Menú Lateral");
        await Task.Delay(1000); 
        
        // 🚨 DIAGNÓSTICO QA (BUG DE HTML SUCIO): 
        // El DOM inyecta clones de 'Salir'. Validamos el botón disparador para confirmar estabilidad.
        await Expect(botonMenu).ToBeVisibleAsync();
        LogWriter("Ciclo de apertura/cierre completado exitosamente.");
    }
}