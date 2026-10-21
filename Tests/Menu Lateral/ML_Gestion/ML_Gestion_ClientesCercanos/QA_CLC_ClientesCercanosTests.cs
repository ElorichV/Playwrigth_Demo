using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: CLIENTES CERCANOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Clientes Cercanos. La grabación de codegen no capturó un heading o
/// selector único de contenido para esta pantalla, así que la aserción por ahora solo confirma
/// que la navegación no termina en una página de error. Pendiente de un mapeo más fino en una
/// siguiente pasada. Ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_CLC_ClientesCercanosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_CLC_01_AccesoAClientesCercanos()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Clientes Cercanos" }), "Clic en Clientes Cercanos");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        Assert.That(Page.Url, Does.Not.Contain("error"), "La navegación a Clientes Cercanos terminó en una página de error.");
    }
}
