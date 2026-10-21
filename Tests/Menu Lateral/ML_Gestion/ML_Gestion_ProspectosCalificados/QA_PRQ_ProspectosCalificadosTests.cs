using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: PROSPECTOS CALIFICADOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Prospectos calificados. La grabación de codegen mostró el mismo heading
/// genérico "Prospectos" que el módulo <c>QA_PRS_ProspectosTests</c> — puede ser la misma
/// pantalla reutilizada con un filtro distinto; pendiente de confirmar si hay una diferencia
/// visual real entre ambas. Selectores confirmados mediante codegen, ver
/// <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_PRQ_ProspectosCalificadosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_PRQ_01_AccesoAProspectosCalificados()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Prospectos calificados" }), "Clic en Prospectos calificados");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Prospectos" })).ToBeVisibleAsync();
    }
}
