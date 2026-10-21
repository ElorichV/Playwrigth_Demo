using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_VelocidadPaginas;

/// <summary>
/// Valida la redirección a Google PageSpeed Insights desde "Velocidad de páginas". El enlace
/// puede abrir pestaña nueva o navegar en la misma pestaña según el ambiente/versión de la app;
/// la prueba tolera ambos casos (ver el try/catch de <c>RunAndWaitForPopupAsync</c> abajo).
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_VelocidadPaginasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        // FIX (codegen): el menu lateral muestra Gestion por defecto. Los links de Ayudas
        // no aparecen en el DOM hasta cambiar a esta seccion - causa del timeout 10s.
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();
    }
    [Test]
    public async Task QA_AYD_RedireccionPageSpeed()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menu");

        // FIX: el enlace dejo de abrir una pestana nueva (target="_blank") y ahora navega en
        // la misma pestana - confirmado con el HAR de esta prueba: Playwright solo registro
        // una sola pagina ("PageSpeed Insights"), nunca una segunda. RunAndWaitForPopupAsync
        // sin mas se quedaba esperando 45s un evento "Popup" que ya no ocurre. Probamos
        // primero si abre pestana nueva (comportamiento historico); si no, asumimos que la
        // redireccion ocurrio en la misma pestana.
        IPage? popupPageSpeed = null;
        try
        {
            popupPageSpeed = await Page.RunAndWaitForPopupAsync(async () =>
            {
                await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Velocidad de páginas" }), "Clic en Velocidad");
            }, new() { Timeout = 5000 });
        }
        catch (System.TimeoutException)
        {
            LogWriter("'Velocidad de paginas' no abrio pestana nueva; validando redireccion en la misma pestana.");
        }

        var destino = popupPageSpeed ?? Page;
        await Expect(destino).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("pagespeed"));

        if (popupPageSpeed != null) await popupPageSpeed.CloseAsync();
    }
}
