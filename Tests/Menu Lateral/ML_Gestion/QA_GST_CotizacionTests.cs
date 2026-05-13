using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Factories;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
public class QA_GST_CotizacionTests : BaseTest
{
    public static IEnumerable<TestCaseData> LeerCasosCotizacion()
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_GST_CotizacionData.json");
        var json = File.ReadAllText(ruta);
        var casos = JsonSerializer.Deserialize<List<CotizacionTestData>>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        foreach (var caso in casos)
        {
            yield return new TestCaseData(caso).SetName($"{caso.CasoId}_{caso.Producto.Replace(" ", "")}");
        }
    }

    private async Task ConfigurarProductoInteligente(CotizacionTestData datos)
    {
        var config = ProductoFactory.ObtenerConfiguracion(datos.Producto);
        if (config == null) Assert.Fail($"Producto '{datos.Producto}' no encontrado en el catálogo.");

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Seleccione Producto" }).ClickAsync();
        await Page.GetByText(datos.Producto).First.ClickAsync();

        if (config.PeriodosValidos.Any())
            await SeleccionarEnSelectize("Seleccione Periodo", datos.Periodo);

        if (config.SuscripcionesValidas.Any())
            await SeleccionarEnSelectize("Selecciones suscripción", datos.Suscripcion);

        await SeleccionarEnSelectize("Seleccione pago", datos.MetodoPago);
    }

    private async Task SeleccionarEnSelectize(string placeholder, string valor)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = placeholder }).ClickAsync();
        await Page.GetByText(valor, new() { Exact = true }).First.ClickAsync();
    }

    [Test]
    [TestCaseSource(nameof(LeerCasosCotizacion))]
    public async Task QA_GST_02_GenerarCotizacion_DataDriven(CotizacionTestData testCase)
    {
        await LoginDinamico();
        var cliente = ClientePoolFactory.ObtenerClienteAleatorio();

        LogWriter($"Cotizando '{testCase.Producto}' para Cliente ID: {cliente.IdCliente}");

        await Page.GotoAsync($"{Config.Url}VistasCompartidas/Cotizador?folio=0&advertiser_id={cliente.IdCliente}");
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Teléfono" }).FillAsync(cliente.Telefono);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Correo" }).FillAsync(cliente.Correo);

        await ConfigurarProductoInteligente(testCase);

        await SeleccionarEnSelectize("Seleccione Estado", cliente.Estado);
        await SeleccionarEnSelectize("Seleccione Municipio", cliente.Municipio);
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Seleccione Categoría" }).FillAsync(cliente.Categoria);
        await Page.GetByText(cliente.Categoria, new() { Exact = false }).First.ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Agregar Producto" }).ClickAsync();
        LogWriter("Cotización completada exitosamente.");
    }
}