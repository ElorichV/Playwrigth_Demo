using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ESTATUS DE PRODUCTOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida las 4 sub-vistas de Estatus de Productos (En producción, Publicados, Cancelados,
/// Proceso cancelación), cada una filtrable por "Libro"/"Digital". Solo "En producción" tuvo una
/// URL propia confirmada en la grabación (<c>/Productos/ProdInProgres</c>); las otras tres se
/// validan por el filtro "Libro" visible, que sí se confirmó en la grabación para cada una.
/// Selectores confirmados mediante codegen, ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_EDP_EstatusProductosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Estatus de Productos" }), "Clic en Estatus de Productos");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
    }

    [Test]
    public async Task QA_EDP_01_EnProduccion()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " En producción" }), "Clic en En producción");
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("ProdInProgres"));
        await Expect(Page.GetByText("Libro").First).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_EDP_02_Publicados()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Publicados" }), "Clic en Publicados");
        await Expect(Page.GetByText("Libro").First).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_EDP_03_Cancelados()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Cancelados" }), "Clic en Cancelados");
        await Expect(Page.GetByText("Libro").First).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_EDP_04_ProcesoCancelacion()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Proceso cancelación" }), "Clic en Proceso cancelación");
        await Expect(Page.GetByText("Libro").First).ToBeVisibleAsync();
    }
}
