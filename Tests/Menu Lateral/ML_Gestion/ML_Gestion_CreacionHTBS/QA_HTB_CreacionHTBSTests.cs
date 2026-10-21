using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: CREACIÓN HTBS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Creación HTBS. La grabación de codegen solo registró el clic en el enlace,
/// sin capturar contenido posterior (heading o selector distintivo), así que la aserción por
/// ahora solo confirma que la navegación no termina en una página de error. Pendiente de un
/// mapeo más fino en una siguiente pasada. Ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_HTB_CreacionHTBSTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_HTB_01_AccesoACreacionHTBS()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.Locator("a").Filter(new() { HasText = "Creación HTBS" }), "Clic en Creación HTBS");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        Assert.That(Page.Url, Does.Not.Contain("error"), "La navegación a Creación HTBS terminó en una página de error.");
    }
}
