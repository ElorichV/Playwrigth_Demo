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
// SUITE DE PRUEBAS: MENÚ LATERAL - TABLERO
// ---------------------------------------------------------
/// <summary>
/// Muestrea enlaces clave del Dashboard para verificar el ruteo interno
/// (ej. Comisiones, Tablero) y valida el renderizado de los componentes destino.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
public class QA_SMK_MenuLateral_Dashboard_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuDashboard()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }

    public static IEnumerable<TestCaseData> LeerLinksTableroMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_SMK_TableroLinksData.json");
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(File.ReadAllText(rutaJson))!;

        string[] idsMuestra = { "QA-SMK-02.1", "QA-SMK-02.4", "QA-SMK-02.6" };
        var muestra = enlacesCompletos.Where(e => idsMuestra.Contains(e.Id));

        foreach (var link in muestra)
        {
            yield return new TestCaseData(link).SetName($"{link.Id}_Navegacion_{link.TextoEnlace.Replace(" ", "")}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksTableroMuestreo))]
    public async Task QA_SMK_02_Barredor_Tablero_Dinamico(LinkTestData datos)
    {
        if (!datos.Habilitado) Assert.Ignore($"[SKIP] {datos.Id}: Enlace '{datos.TextoEnlace}' marcado como inactivo en la configuración.");

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");
        
        var opcionMenu = Page.Locator($"a:has-text('{datos.TextoEnlace}')").First;
        await ClickConMonitoreo(opcionMenu, $"Navegación a {datos.TextoEnlace}");

        LogWriter($"Verificando renderizado en destino: {datos.SelectorValidacion}");
        await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

        // Limpieza de estado: Retorno seguro
        var botonRegresar = Page.Locator("#back, #back a").First;
        if (await botonRegresar.IsVisibleAsync()) 
        {
            await ClickConMonitoreo(botonRegresar, "Botón Regresar");
        }
    }
}