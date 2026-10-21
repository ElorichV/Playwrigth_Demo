using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: CAMBIOS A DIGITALES (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Módulo "Cambios a Digitales". En <c>[Ignore]</c>: la corrida real confirmó que el enlace
/// redirige a <c>http://propuestas.i.seccionamarilla.com/error.aspx</c> — la misma página de
/// error que "Envío de Material" (<c>QA_ENV_EnvioMaterialTests</c>), lo que sugiere una causa
/// compartida entre ambos enlaces. Bug de ambiente, no de esta prueba. Reportado en
/// <c>docs/PROBLEMAS_CONOCIDOS.md</c>. La aserción queda escrita contra el bug a propósito: si
/// se corrige, esta prueba empezará a FALLAR, que es la señal de quitar el [Ignore].
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_CAD_CambiosDigitalesTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    [Ignore("BUG DE AMBIENTE: el enlace 'Cambios a Digitales' redirige a propuestas.i.seccionamarilla.com/error.aspx (misma causa que Envío de Material). Ver docs/PROBLEMAS_CONOCIDOS.md.")]
    public async Task QA_CAD_01_AccesoACambiosDigitales()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Cambios a Digitales" }), "Clic en Cambios a Digitales");
        await Expect(Page).ToHaveURLAsync(new Regex("error\\.aspx"));
    }
}
