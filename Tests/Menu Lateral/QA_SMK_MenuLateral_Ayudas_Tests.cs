using Microsoft.Playwright;
using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
public class QA_SMK_MenuLateral_Ayudas_Tests : BaseTest
{
    public static IEnumerable<TestCaseData> LeerLinksAyudaMuestreo()
    {
        // 🔗 RUTA ACTUALIZADA
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/QA_SMK_AyudasLinksData.json");
        var contenidoJson = File.ReadAllText(rutaJson);
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(contenidoJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        string[] idsMuestra = { "QA-SMK-04.1", "QA-SMK-04.5", "QA-SMK-04.11" };
        var muestra = enlacesCompletos.Where(link => idsMuestra.Contains(link.Id));

        foreach (var link in muestra)
        {
            yield return new TestCaseData(link).SetName($"{link.Id}_{link.TextoEnlace.Replace(" ", "_")}");
        }
    }

    [SetUp]
    public async Task SetupAyudas()
    {
        await LoginDinamico();
        LogWriter("Configurando estado base: Modo Ayudas.");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksAyudaMuestreo))]
    public async Task QA_SMK_04_Barredor_Ayudas_Dinamico(LinkTestData datos)
    {
        LogWriter($"Iniciando Smoke Test: {datos.Id}");
        try 
        {
            await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");
            await Task.Delay(800); 

            var opcionMenu = Page.Locator($"a:has-text('{datos.TextoEnlace}')").First;

            if (datos.EsExterno)
            {
                var popup = await Page.RunAndWaitForPopupAsync(async () => {
                    await ClickConMonitoreo(opcionMenu, $"Clic Externo {datos.TextoEnlace}");
                });
                await popup.CloseAsync();
            }
            else
            {
                await ClickConMonitoreo(opcionMenu, $"Navegación Interna {datos.TextoEnlace}");
                await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

                // --- HACK DE RESILIENCIA: Reset forzado para refrescar el DOM ---
                LogWriter("[HACK] Reset de Radio Buttons para refrescar la interfaz.");
                var btnRegresar = Page.Locator("#back, #back a").First;
                await ClickConMonitoreo(btnRegresar, "Botón Regresar");
                await Page.GetByRole(AriaRole.Radio, new() { Name = "Dashboard" }).CheckAsync();
                await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();
            }
            LogWriter($"Prueba {datos.Id} finalizada.");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Fallo crítico en {datos.Id}: {ex.Message}");
        }
    }
}