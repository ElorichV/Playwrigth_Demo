using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ENTREVISTA (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Entrevista (heading "Entrevista Pinbox"). Selectores confirmados mediante
/// codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_ENT_EntrevistaTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_ENT_01_AccesoAEntrevista()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Entrevista" }), "Clic en Entrevista");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Entrevista Pinbox" })).ToBeVisibleAsync();
    }
}
