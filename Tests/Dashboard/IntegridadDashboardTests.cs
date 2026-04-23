using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
public class IntegridadDashboardTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await Page.AddLocatorHandlerAsync(
            Page.GetByRole(AriaRole.Button, new() { Name = "Continuar navegando." }),
            async () => await Page.GetByRole(AriaRole.Button, new() { Name = "Continuar navegando." }).ClickAsync(new() { Force = true })
        );

        await Page.AddLocatorHandlerAsync(
            Page.GetByText("La búsqueda ingresada no contiene información"),
            async () => await Page.GetByRole(AriaRole.Button, new() { Name = "Aceptar" }).ClickAsync(new() { Force = true })
        );

        await LoginDinamico("sashenka.olais", "P1nB0x");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    public static IEnumerable<UsuarioTestData> LeerDatosDashboard()
    {
        var json = File.ReadAllText("TestData/usuarios.json");
        var todosLosDatos = JsonSerializer.Deserialize<List<UsuarioTestData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        return todosLosDatos.Where(dato => dato.DebeSerExitoso == true);
    }

    [Test]
    [TestCaseSource(nameof(LeerDatosDashboard))]
    public async Task TC04_ValidarIdentidadEnDashboard(UsuarioTestData datos) 
    {
        var locatorAgente = Page.Locator($"text={datos.AgenteEsperado} >> visible=true");
        await Expect(locatorAgente).ToBeVisibleAsync();
    }

    // --------------------------------------------------------------
    // TC05 - Carga de KPI Base (Presupuesto, Solicitudes, Alcance)
    // --------------------------------------------------------------
    [Test]
    public async Task TC05_ValidarCargaDeKPIBase()
    {
        var inputPresupuesto = Page.Locator("#pptoComercial");
        var inputSolicitudes = Page.Locator("#solictudesComercial");
        var inputAlcance = Page.Locator("#avanceComercial");

        await Expect(inputPresupuesto).ToBeVisibleAsync(new() { Timeout = 15000 });
        await Expect(inputSolicitudes).ToBeVisibleAsync(new() { Timeout = 15000 });
        await Expect(inputAlcance).ToBeVisibleAsync(new() { Timeout = 15000 });

        await Expect(inputPresupuesto).Not.ToBeEmptyAsync();
        await Expect(inputSolicitudes).Not.ToBeEmptyAsync();
        await Expect(inputAlcance).Not.ToBeEmptyAsync();
    }

    // --------------------------------------------------------------
    // TC06 - Interactividad de Tarjetas de Estado (Círculos Azules)
    // --------------------------------------------------------------
    [Test]
    public async Task TC06_ValidarTarjetasDeEstado()
    {
        var tarjetas = new Dictionary<string, string>
        {
            { "#btnComercialNo", "No Elaborado" },
            { "#btnComercialPendiente", "Pendiente" },
            { "#btnComercialCancelada", "Cancelada" },
            { "#btnComercialAbierta", "Abierta" },
            { "#btnComercialPosteado", "Posteado" }
        };

        foreach (var tarjeta in tarjetas)
        {
            await Page.Locator(tarjeta.Key).ClickAsync();
            
            var tituloModal = Page.GetByRole(AriaRole.Heading).Filter(new() { HasText = tarjeta.Value }).First;
            await Expect(tituloModal).ToBeVisibleAsync(new() { Timeout = 10000 });
            
            var btnCerrar = Page.GetByRole(AriaRole.Button, new() { Name = "X" }).First;
            await btnCerrar.ClickAsync(new() { Force = true });
            // LA CURA: Obligamos al robot a esperar a que la "X" desaparezca visualmente
            await Expect(btnCerrar).ToBeHiddenAsync(new() { Timeout = 5000 });
        }
    }
}