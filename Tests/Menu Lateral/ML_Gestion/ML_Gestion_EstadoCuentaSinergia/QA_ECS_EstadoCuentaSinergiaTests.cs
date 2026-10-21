using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ESTADO DE CUENTA SINERGIA (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Estado de Cuenta Sinergia (campo de folio <c>#numeroFolio</c>). Selectores
/// confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_ECS_EstadoCuentaSinergiaTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_ECS_01_AccesoAEstadoDeCuenta()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Estado de Cuenta Sinergia" }), "Clic en Estado de Cuenta Sinergia");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.Locator("#numeroFolio")).ToBeVisibleAsync();
    }
}
