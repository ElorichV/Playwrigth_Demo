using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: CONTRATOS CREADOS (Módulo 4.3 — Gestión)
// ---------------------------------------------------------
/// <summary>
/// Módulo "Contratos Creados": tabla (grid) de contratos por cliente, con filas expandibles
/// (+/-) hacia el detalle de cada contrato. Ruta real: menú Gestión → " Contratos creados" →
/// <c>/Cliente/ContratosAgente</c> (selectores confirmados vía <c>codegen</c>, ver
/// <c>docs/codegen_pantallas_gestion.txt</c>).
///
/// Sigue en <c>[Ignore]</c>: la grabación de codegen usó la cuenta genérica de sesión y sí
/// mostró datos (cliente "CAJAS FUERTES OFFITECH"), pero falta confirmar con una corrida real
/// si <c>LoginDinamico</c> siempre trae contratos o si, igual que Cotizaciones/Clientes, el
/// contenido depende del cliente/agente logueado (en cuyo caso tocaría <c>LoginEspecifico</c>).
/// No se afirma contra datos de cliente específicos porque son datos de ambiente, no fijos.
/// </summary>
[TestFixture]
[Category("Gestion")]
[Category("Contratos")]
public class QA_CTR_ContratosCreadosTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
    }

    [Test]
    [Ignore("PENDIENTE DE CONFIRMAR: falta validar en una corrida real si LoginDinamico siempre trae contratos o si requiere LoginEspecifico (ver docs/codegen_pantallas_gestion.txt).")]
    public async Task QA_CTR_01_AccesoAlListado()
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Contratos creados" }), "Clic en Contratos creados");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Expect(Page.GetByRole(AriaRole.Grid).First).ToBeVisibleAsync();
    }
}
