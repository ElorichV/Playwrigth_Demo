using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: DOCUMENTOS (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Valida el acceso a Documentos: un visor Docukeeper embebido en el iframe
/// <c>#frmDocSinergia</c> con la sección "PRE FOLIO". Selectores confirmados mediante codegen,
/// ver <c>docs/codegen_pantallas_gestion.txt</c>.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_DOC_DocumentosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    public async Task QA_DOC_01_AccesoADocumentos()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Documentos" }), "Clic en Documentos");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        var frame = Page.Locator("#frmDocSinergia").ContentFrame;
        await Expect(frame.GetByRole(AriaRole.Heading, new() { Name = "PRE FOLIO" })).ToBeVisibleAsync();
    }
}
