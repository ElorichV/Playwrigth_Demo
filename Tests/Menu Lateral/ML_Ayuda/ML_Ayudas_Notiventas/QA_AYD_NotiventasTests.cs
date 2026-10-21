using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Notiventas;

/// <summary>
/// Valida que Notiventas abra su contenido en una pestaña nueva. Si esta prueba empieza a fallar
/// con "Timeout ... waiting for event Popup", revisa primero si el enlace dejó de abrir pestaña
/// nueva y pasó a navegar en la misma — es justo lo que le pasó a "Velocidad de páginas" en
/// ML_Ayudas_VelocidadPaginas/QA_AYD_VelocidadPaginasTests.cs, que ya tiene el patrón de
/// fallback (intentar popup, si no aparece en unos segundos asumir navegación en la misma
/// pestaña) resuelto para copiar aquí si hace falta.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_NotiventasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        // 🚨 FIX (codegen): el menú lateral muestra Gestión por defecto. Los links de Ayudas
        // no aparecen en el DOM hasta cambiar a esta sección — causa del timeout 10s.
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();
    }
    [Test]
    public async Task QA_AYD_AperturaNoticiaEnNuevaPestana()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.Locator("#menu_notiventas"), "Clic en Notiventas");

        // Manejo de Pop-up (Nueva pestaña)
        var nuevaPestana = await Page.RunAndWaitForPopupAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Cell).First.ClickAsync();
        });

        Assert.That(nuevaPestana, Is.Not.Null, "El documento de Notiventas no se abrió.");
        await nuevaPestana.CloseAsync(); // Cerramos la pestaña generada

        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}