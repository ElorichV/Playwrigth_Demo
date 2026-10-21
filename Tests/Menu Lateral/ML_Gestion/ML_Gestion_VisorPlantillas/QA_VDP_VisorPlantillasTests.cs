using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: VISOR DE PLANTILLAS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso al Visor de Plantillas: un catálogo de plantillas de sitio (por categoría,
/// ej. "Animales y mascotas") mostrado dentro de un iframe (<c>my_iframe</c>). Selectores
/// confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_VDP_VisorPlantillasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_VDP_01_AccesoAlCatalogoDePlantillas()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Visor de plantillas" }), "Clic en Visor de plantillas");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        var frame = Page.Locator("iframe[name=\"my_iframe\"]").ContentFrame;
        await Expect(frame.GetByRole(AriaRole.Link, new() { Name = "Animales y mascotas" })).ToBeVisibleAsync();
    }
}
