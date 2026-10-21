using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: CARTERA VENCIDA (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Cartera vencida. La grabación de codegen mostró que este flujo dispara
/// diálogos nativos del navegador (<c>window.confirm</c>/<c>alert</c>), por eso la prueba
/// suscribe un handler de <see cref="IPage.Dialog"/> que los descarta automáticamente antes de
/// hacer clic. Termina navegando a <c>/Agente/ClientesGestion</c>. Selectores confirmados
/// mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_CTV_CarteraVencidaTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_CTV_01_AccesoAClienteGestion()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        Page.Dialog += async (_, dialog) => await dialog.DismissAsync();

        await ClickConMonitoreo(Page.Locator("a").Filter(new() { HasText = "Cartera vencida" }), "Clic en Cartera vencida");
        await Expect(Page).ToHaveURLAsync(new Regex("ClientesGestion"));
    }
}
