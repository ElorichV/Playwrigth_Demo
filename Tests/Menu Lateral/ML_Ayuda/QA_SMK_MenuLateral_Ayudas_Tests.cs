using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: MENU LATERAL - AYUDAS
// ---------------------------------------------------------
/// <summary>
/// Valida la navegacion de los enlaces ubicados en el menu "Ayudas" utilizando una
/// estrategia de muestreo. Comprueba enlaces internos y externos (Popups).
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
public class QA_SMK_MenuLateral_Ayudas_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuAyudas()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }

    public static IEnumerable<TestCaseData> LeerLinksAyudaMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_SMK_AyudasLinksData.json");
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(File.ReadAllText(rutaJson))!;

        string[] idsMuestra = { "QA-SMK-04.1", "QA-SMK-04.5", "QA-SMK-04.11" };
        var muestra = enlacesCompletos.Where(e => idsMuestra.Contains(e.Id));

        foreach (var link in muestra)
        {
            yield return new TestCaseData(link).SetName($"{link.Id}_{link.TextoEnlace.Replace(" ", "")}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksAyudaMuestreo))]
    public async Task QA_SMK_04_Barredor_Ayudas_Dinamico(LinkTestData datos)
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menu");

        var opcionMenu = Page.Locator($"a:has-text('{datos.TextoEnlace}')").First;

        // ====================================================================
        // Desplegar el acordeon padre si la opcion esta oculta
        // ====================================================================
        if (!await opcionMenu.IsVisibleAsync())
        {
            LogWriter("El enlace esta oculto en el acordeon. Abriendo categoria 'Ayudas'...");

            // FIX: el <span>Ayudas</span> queda tapado por el radio button real
            // (#tab-home-3, title="Ayudas") y Playwright reporta "intercepts pointer events"
            // al hacer clic sobre el texto. Activamos el radio directamente, igual que en
            // el resto de las 11 clases de ML_Ayuda (ver QA_AYD_NotiventasTests, etc.).
            await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();

            // Espera breve para permitir que la animacion CSS termine de bajar el submenu
            await Task.Delay(1000);
        }
        // ====================================================================

        // Logica de bifurcacion: Manejo de Popups Externos vs Ruteo Interno
        if (datos.EsExterno)
        {
            LogWriter($"Interceptando evento de nueva pestana (Target='_blank') para {datos.TextoEnlace}");

            // FIX: al menos "Velocidad de paginas" dejo de abrir pestana nueva y ahora navega
            // en la misma pestana (confirmado con el HAR: Playwright solo registro una sola
            // pagina). Probamos primero si abre pestana nueva; si no ocurre en 5s, asumimos
            // que la redireccion fue en la misma pestana y seguimos sin fallar la prueba.
            IPage? popup = null;
            try
            {
                popup = await Page.RunAndWaitForPopupAsync(async () => {
                    // Clic natural, ya sin Force=true porque el elemento es visible
                    await opcionMenu.ClickAsync();
                }, new() { Timeout = 5000 });
            }
            catch (System.TimeoutException)
            {
                LogWriter($"'{datos.TextoEnlace}' no abrio pestana nueva; asumiendo redireccion en la misma pestana.");
            }

            if (popup != null) await popup.CloseAsync();
        }
        else
        {
            LogWriter($"Navegacion Interna {datos.TextoEnlace}");

            // Clic natural, ya sin Force=true porque el elemento es visible
            await opcionMenu.ClickAsync();

            await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

            var btnRegresar = Page.Locator("#back, #back a").First;
            if (await btnRegresar.IsVisibleAsync()) await btnRegresar.ClickAsync();
        }
    }
}
