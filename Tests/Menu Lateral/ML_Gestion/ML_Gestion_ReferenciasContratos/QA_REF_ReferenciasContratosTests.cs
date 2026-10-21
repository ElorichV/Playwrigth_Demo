using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: REFERENCIAS DE CONTRATOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Referencias de contratos (heading "Referencias contrato"). Selectores
/// confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_REF_ReferenciasContratosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_REF_01_AccesoAReferenciasDeContratos()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Referencias de contratos" }), "Clic en Referencias de contratos");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Referencias contrato" })).ToBeVisibleAsync();
    }
}
