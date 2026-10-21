using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: BUSCADOR DE CLIENTE (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso al Buscador de clientes (formulario con Nombre comercial, R.F.C., Teléfono,
/// E-mail, C.P., etc.). Selectores confirmados mediante codegen, ver
/// <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_BUS_BuscadorClienteTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_BUS_01_AccesoAlBuscador()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Buscador cliente" }), "Clic en Buscador cliente");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Buscador de clientes" })).ToBeVisibleAsync();
    }
}
