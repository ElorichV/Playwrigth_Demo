using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: MOVIMIENTOS DE CAMPAÑA (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Movimientos de campaña (buscador por "Nombre comercial o Núm Cliente").
/// Selectores confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_MDC_MovimientosCampanaTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_MDC_01_AccesoAlBuscadorDeMovimientos()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Movimientos de campaña" }), "Clic en Movimientos de campaña");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = " Nombre comercial o Núm" })).ToBeVisibleAsync();
    }
}
