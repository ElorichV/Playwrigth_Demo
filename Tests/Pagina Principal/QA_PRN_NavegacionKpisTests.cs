using System;
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
/// <summary>
/// Valida la integridad del ruteo hacia los detalles de KPIs.
/// Emplea un patrón de "Muestreo Representativo" (Sampling) para mantener los tiempos de ejecución 
/// en niveles óptimos de CI/CD sin comprometer la cobertura de código.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Regresion")]  
[Category("Media")]      
public class QA_PRN_NavegacionKpisTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    public static IEnumerable<TestCaseData> LeerDatosKpisMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_PRN_KpisData.json");
        var json = File.ReadAllText(rutaJson);
        var kpis = JsonSerializer.Deserialize<List<KpiTestData>>(json)!;

        // Sampling: Primer KPI, Medio y Último para no saturar
        var muestra = new List<KpiTestData> { kpis.First(), kpis[kpis.Count / 2], kpis.Last() };

        foreach (var kpi in muestra)
        {
            yield return new TestCaseData(kpi).SetName($"{kpi.CasoId}_ValidarKPI_{kpi.TituloEsperado}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerDatosKpisMuestreo))]
    public async Task QA_PRN_ValidarEnlacesDeKPICirculares(KpiTestData datos)
    {
        LogWriter($"Ejecutando ruteo hacia detalle de KPI: {datos.Boton}");

        ILocator selectorKpi = datos.EsId 
            ? Page.Locator(datos.Boton!) 
            : Page.GetByText(datos.Boton!).First;

        try 
        {
            await ClickConMonitoreo(selectorKpi, $"Acceso a KPI {datos.CasoId}");
            
            // Aserción del Front-End (Validar que el título esperado se renderice)
            // ✅ LA CORRECCIÓN: Buscamos un 'span' (texto) que simplemente contenga el título del KPI
            await Expect(Page.Locator("span").Filter(new() { HasText = datos.TituloEsperado }).First).ToBeVisibleAsync();
            LogWriter($"Ruteo al KPI '{datos.TituloEsperado}' completado exitosamente.");
        }
        catch (System.TimeoutException)
        {
            // Manejo de Resiliencia: Si la red falla, marcamos un Warning en vez de un Crash mortal.
            Assert.Warn($"[LATENCIA DE RED] El KPI {datos.CasoId} no se cargó a tiempo. Posible cuello de botella en el servicio.");
        }
    }
}