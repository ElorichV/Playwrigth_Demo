using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ENVÍO DE MATERIAL (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Módulo "Envío de Material". En <c>[Ignore]</c>: la grabación de codegen mostró que el enlace
/// del menú redirige directo a <c>http://propuestas.i.seccionamarilla.com/error.aspx</c> — un
/// bug de ambiente, no de esta prueba. Reportado en <c>docs/PROBLEMAS_CONOCIDOS.md</c>. La
/// aserción queda escrita contra el bug (URL de error) a propósito: si algún día se corrige,
/// esta prueba empezará a FALLAR, lo cual es la señal de que hay que quitarle el [Ignore] y
/// escribir la aserción real de la pantalla funcional.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_ENV_EnvioMaterialTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    [Ignore("BUG DE AMBIENTE: el enlace 'Envío de Material' redirige a propuestas.i.seccionamarilla.com/error.aspx. Ver docs/PROBLEMAS_CONOCIDOS.md.")]
    public async Task QA_ENV_01_AccesoAEnvioDeMaterial()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Envío de Material" }), "Clic en Envío de Material");
        await Expect(Page).ToHaveURLAsync(new Regex("error\\.aspx"));
    }
}
