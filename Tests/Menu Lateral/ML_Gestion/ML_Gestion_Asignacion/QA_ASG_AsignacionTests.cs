using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ASIGNACIÓN (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Asignación y su panel de filtros ("Objetivo" + botón "Aplicar Filtros").
/// Selectores confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_ASG_AsignacionTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_ASG_01_AccesoAAsignacionYFiltros()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Asignación" }), "Clic en Asignación");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByLabel("Objetivo")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = " Aplicar Filtros" })).ToBeVisibleAsync();
    }
}
