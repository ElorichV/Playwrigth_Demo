using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: VISTA PANORÁMICA (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Vista Panorámica. Selectores confirmados mediante codegen, ver
/// <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_VSP_VistaPanoramicaTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_VSP_01_AccesoAVistaPanoramica()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByTitle("Vista Panorámica"), "Clic en Vista Panorámica");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.Locator("div").Filter(new() { HasText = "Vista Panorámica." }).First).ToBeVisibleAsync();
    }
}
