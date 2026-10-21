using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: PROSPECTOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso al listado de Prospectos. Usa <c>Exact = true</c> en el enlace del menú
/// porque "Prospectos" es substring de otros enlaces del mismo menú ("Prospectos TMX",
/// "Prospectos calificados") — mismo criterio que exige <c>CLAUDE.md</c> para selectores por
/// texto ambiguos. Selectores confirmados mediante codegen, ver
/// <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_PRS_ProspectosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_PRS_01_AccesoAlListadoDeProspectos()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Prospectos", Exact = true }), "Clic en Prospectos");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Prospectos" })).ToBeVisibleAsync();
    }
}
