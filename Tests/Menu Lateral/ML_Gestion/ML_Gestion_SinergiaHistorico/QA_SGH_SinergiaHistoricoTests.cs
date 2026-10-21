using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: SINERGIA HISTÓRICO (Gestión)
// ---------------------------------------------------------
/// <summary>
/// Módulo "Sinergia histórico". En <c>[Ignore]</c>: la grabación de codegen mostró, en dos
/// visitas separadas a esta pantalla, el mismo diálogo de error ("No se puede mostrar
/// información... Intente nuevamente por favor") en vez del contenido esperado — bug de
/// ambiente, no de esta prueba. Reportado en <c>docs/PROBLEMAS_CONOCIDOS.md</c>. La aserción
/// queda escrita contra el bug a propósito: si se corrige, esta prueba empezará a FALLAR, que es
/// la señal de quitar el [Ignore] y escribir la aserción real del contenido histórico.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Funcional")]
public class QA_SGH_SinergiaHistoricoTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    [Ignore("BUG DE AMBIENTE: 'Sinergia histórico' muestra el diálogo 'No se puede mostrar información' en vez de cargar el contenido, reproducido en 2 corridas. Ver docs/PROBLEMAS_CONOCIDOS.md.")]
    public async Task QA_SGH_01_AccesoASinergiaHistorico()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Sinergia histórico" }), "Clic en Sinergia histórico");
        await Expect(Page.GetByRole(AriaRole.Dialog, new() { Name = "No se puede mostrar informaci" })).ToBeVisibleAsync();
    }
}
