using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models; 

namespace Playwrigt_Demo;

[TestFixture]
public class QA_SMK_MenuLateral_Dashboard_Tests : BaseTest
{
    public static IEnumerable<TestCaseData> LeerLinksTableroMuestreo()
    {
        // 🔗 RUTA ACTUALIZADA
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/QA_SMK_TableroLinksData.json");
        var contenidoJson = File.ReadAllText(rutaJson);
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(contenidoJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // ESTRATEGIA DE SELECCIÓN (Muestreo Crítico)
        string[] idsMuestra = { "QA-SMK-02.1", "QA-SMK-02.4", "QA-SMK-02.6" };
        var muestra = enlacesCompletos.Where(link => idsMuestra.Contains(link.Id));

        foreach (var link in muestra)
        {
            yield return new TestCaseData(link)
                .SetName($"{link.Id}_{link.TextoEnlace.Replace(" ", "_")}");
        }
    }

    [SetUp]
    public async Task SetupRedirecciones()
    {
        await LoginDinamico();
        LogWriter("Estableciendo Pestaña 1 como estado base.");
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksTableroMuestreo))]
    public async Task QA_SMK_02_Barredor_Tablero_Dinamico(LinkTestData datos)
    {
        LogWriter($"Iniciando validación: {datos.Id} - {datos.TextoEnlace}");

        if (!datos.Habilitado)
        {
            Assert.Ignore($"[SKIP] {datos.Id}: Enlace no operativo.");
        }

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");
        await Task.Delay(800); 

        try 
        {
            var opcionMenu = Page.Locator($"a:has-text('{datos.TextoEnlace}')").First;
            await ClickConMonitoreo(opcionMenu, $"Navegación a {datos.TextoEnlace}");
            await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

            LogWriter($"Verificando Heading: {datos.SelectorValidacion}");
            await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

            // RETORNO SEGURO
            var botonRegresar = Page.Locator("#back, #back a").First;
            await ClickConMonitoreo(botonRegresar, "Botón Regresar");
            await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        }
        catch (System.Exception ex)
        {
            LogWriter($"[ERROR] Fallo en {datos.Id}: {ex.Message}");
            Assert.Fail($"Fallo en {datos.Id}: {ex.Message}");
        }
    }
}