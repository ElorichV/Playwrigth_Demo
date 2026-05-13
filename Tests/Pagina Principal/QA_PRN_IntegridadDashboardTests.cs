using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTEGRIDAD DE DATOS (PÁGINA PRINCIPAL / DASHBOARD)
// ---------------------------------------------------------
// Descripción: Valida que la información estática y dinámica del Dashboard principal 
// (KPIs, identidad del usuario, paneles de estado) se cargue correctamente tras el login.
[TestFixture]
public class QA_PRN_IntegridadDashboardTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------
    // ALIMENTADOR DE DATOS (Data Provider Especializado)
    // ---------------------------------------------------------
    public static IEnumerable<TestCaseData> LeerDatosDashboard()
    {
        // 🔗 RUTA ACTUALIZADA A LA NUEVA NOMENCLATURA
        var json = File.ReadAllText("../../../TestData/QA_LGN_Data.json");
        var todosLosDatos = JsonSerializer.Deserialize<List<UsuarioTestData>>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        
        // Asignamos el filtro a la variable en lugar de retornarlo de golpe
        var datosFiltrados = todosLosDatos.Where(dato => dato.DebeSerExitoso == true);

        foreach (var dato in datosFiltrados)
        {
            // Retornamos cada caso individualmente con su nuevo nombre limpio
            yield return new TestCaseData(dato)
                .SetName($"QA_PRN_12_ValidarIdentidadEnDashboard_Usuario_{dato.Usuario.Replace(".", "_")}");
        }
    }

    // ---------------------------------------------------------
    // QA-PRN-12 - Identidad Dinámica en Dashboard
    // ---------------------------------------------------------
    // Descripción: Valida que el nombre del Agente mostrado en la interfaz coincida 
    // exactamente con el usuario autenticado para evitar cruces de sesión.
    [Test]
    [TestCaseSource(nameof(LeerDatosDashboard))]
    public async Task QA_PRN_12_ValidarIdentidadEnDashboard(UsuarioTestData datos) 
    {
        LogWriter($"Verificando identidad en UI para el Agente: {datos.AgenteEsperado}");
        var locatorAgente = Page.Locator($"text={datos.AgenteEsperado} >> visible=true");
        await Expect(locatorAgente).ToBeVisibleAsync();
        LogWriter("Identidad confirmada.");
    }

    // ---------------------------------------------------------
    // QA-PRN-01 - Carga de KPI Base (Presupuesto, Solicitudes, Alcance)
    // ---------------------------------------------------------
    // Descripción: Comprueba que los campos principales del reporte no solo existan, 
    // sino que contengan datos (confirmando la conexión exitosa a la base de datos).
    [Test]
    public async Task QA_PRN_01_ValidarCargaDeKPIBase()
    {
        LogWriter("Validando carga de datos desde la base de datos en KPIs principales.");
        
        var inputs = new[] { "#pptoComercial", "#solictudesComercial", "#avanceComercial" };

        foreach (var selector in inputs)
        {
            var elemento = Page.Locator(selector);
            await Expect(elemento).ToBeVisibleAsync();
            
            // Verificamos que el campo no esté vacío tras la carga de la página.
            await Expect(elemento).Not.ToBeEmptyAsync();
            LogWriter($"Integridad de datos confirmada para el campo: {selector}");
        }
    }

    // ---------------------------------------------------------
    // QA-PRN-16 - Interactividad de Tarjetas de Estado (Círculos Azules)
    // ---------------------------------------------------------
    // Descripción: Verifica que los botones circulares de métricas respondan a la 
    // interacción sin provocar errores de renderizado en el Front-End.
    [Test]
    public async Task QA_PRN_16_ValidarTarjetasDeEstado()
    {
        LogWriter("Iniciando validación de tarjetas de estado (Dashboard interactivo).");
        
        // Muestreo controlado de 2 tarjetas para optimizar el tiempo de ejecución.
        var tarjetas = new Dictionary<string, string>
        {
            { "#btnComercialNo", "No Elaborado" },
            { "#btnComercialPendiente", "Pendiente" }
        };

        foreach (var tarjeta in tarjetas)
        {
            LogWriter($"Interactuando con tarjeta de estado: {tarjeta.Value}");
            
            // 🛡️ MODO SUPERVIVENCIA: Si el servidor tarda en responder por las APIs internas,
            // el monitor de latencia registrará el evento para el equipo de sistemas.
            await ClickConMonitoreo(Page.Locator(tarjeta.Key), $"Tarjeta {tarjeta.Value}");
            
            // Confirmamos que el Dashboard sigue visible y funcional tras el clic.
            await Expect(Page.Locator("#pptoComercial")).ToBeVisibleAsync();
            await Task.Delay(1000); 
        }
        LogWriter("Validación de tarjetas de estado finalizada.");
    }
}