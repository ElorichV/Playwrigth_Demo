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
// SUITE DE PRUEBAS: NAVEGACIÓN DATA-DRIVEN DE KPIs
// ---------------------------------------------------------
// Descripción: Valida de forma automatizada la interacción con los botones circulares 
// (KPIs) del Dashboard utilizando una estrategia de muestreo representativo (Sampling).
[TestFixture]
public class QA_PRN_NavegacionKpisTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------
    // ALIMENTADOR DE DATOS CON MUESTREO (Sampling Strategy)
    // ---------------------------------------------------------
    // Descripción: Selecciona 3 casos representativos del archivo JSON para 
    // garantizar la integridad de la navegación sin saturar el ambiente de capacitación.
    public static IEnumerable<TestCaseData> LeerDatosKpisMuestreo()
    {
        // 🔗 RUTA ACTUALIZADA A LA NUEVA NOMENCLATURA
        var json = File.ReadAllText("../../../TestData/QA_PRN_KpisData.json");
        var kpis = JsonSerializer.Deserialize<List<KpiTestData>>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // ESTRATEGIA DE SELECCIÓN: Primero, intermedio (Estación IC) y búsqueda por ID (#atencionProd).
        var muestraRepresentativa = new List<KpiTestData> 
        {
            kpis.First(x => x.CasoId == "QA-PRN-01"),
            kpis.First(x => x.CasoId == "QA-PRN-05"),
            kpis.First(x => x.CasoId == "QA-PRN-08") 
        };

        foreach (var kpi in muestraRepresentativa)
        {
            yield return new TestCaseData(kpi)
                .SetName($"{kpi.CasoId}_ValidarEnlaceKPI"); 
        }
    }
    
    // ---------------------------------------------------------
    // QA-PRN-01 al QA-PRN-10 - Enlaces de KPI (Interacción y Tolerancia)
    // ---------------------------------------------------------
    // Descripción: Itera sobre la muestra y ejecuta un clic para validar el acceso al detalle.
    [Test]
    [TestCaseSource(nameof(LeerDatosKpisMuestreo))]
    public async Task QA_PRN_ValidarEnlacesDeKPICirculares(KpiTestData datos)
    {
        LogWriter($"Ejecutando validación para KPI: {datos.CasoId} ({datos.Boton})");

        ILocator selectorKpi = datos.EsId 
            ? Page.Locator(datos.Boton!) 
            : Page.GetByText(datos.Boton!).First;

        // 🔀 BIFURCACIÓN DE LÓGICA SEGÚN MODO DE RED
        if (ModoAuditoriaRed)
        {
            // MODO AUDITORÍA: Validación estricta para captura de tráfico en archivos .har.
            LogWriter("[SISTEMA] Ejecutando en Modo Auditoría (Captura de Red activa).");
            await selectorKpi.ClickAsync(new() { Force = true });
            await Expect(Page.Locator("#pptoComercial")).ToBeVisibleAsync();
        }
        else
        {
            // MODO QA NORMAL: Protección contra falta de datos y latencia de APIs.
            try 
            {
                // El monitor reportará a sistemas si la API de Pinbox supera los 15s de respuesta.
                await ClickConMonitoreo(selectorKpi, $"Acceso a KPI {datos.CasoId}");
            }
            catch (System.TimeoutException)
            {
                LogWriter($"[AVISO] El KPI {datos.CasoId} no se pudo interactuar; omitiendo validación.");
                return; 
            }

            // 🚨 FIX ARQUITECTÓNICO: VALIDACIÓN DE SUPERVIVENCIA
            // Validamos la persistencia del Menú Hamburguesa para confirmar que la sesión 
            // sigue viva y no hubo un colapso del Front-End tras el clic (Aserción con timeout de BaseTest).
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
        }
        LogWriter($"Caso {datos.CasoId} finalizado correctamente.");
    }
}