using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ESCÁNER ADN+ (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso al Escáner ADN+: el enlace abre una pestaña emergente (popup) con un iframe
/// que muestra una tabla de resultados (celda "adn plus" confirmada en la grabación). Selectores
/// confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_ADN_EscanerADNTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_ADN_01_AperturaDePopupYResultados()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        var popup = await Page.RunAndWaitForPopupAsync(async () =>
        {
            await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " escáner ADN+" }), "Clic en Escáner ADN+");
        });

        var frame = popup.Locator("iframe").First.ContentFrame;
        await Expect(frame.GetByRole(AriaRole.Cell, new() { Name = "adn plus" })).ToBeVisibleAsync();

        await popup.CloseAsync();
    }
}
