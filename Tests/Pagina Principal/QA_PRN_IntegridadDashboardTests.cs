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
// SUITE DE PRUEBAS: INTEGRIDAD DE DATOS (DASHBOARD)
// ---------------------------------------------------------
/// <summary>
/// Valida que la información vital del Dashboard principal (KPIs, identidad del usuario, paneles de estado) 
/// se renderice correctamente en el DOM tras la resolución del inicio de sesión.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]      
[Category("Alta")]       
public class QA_PRN_IntegridadDashboardTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    public static IEnumerable<TestCaseData> LeerDatosDashboard()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_LGN_Data.json");
        var json = File.ReadAllText(rutaJson);
        var usuarios = JsonSerializer.Deserialize<List<UsuarioTestData>>(json)!;
        
        var usuarioExitoso = usuarios.FirstOrDefault(u => u.DebeSerExitoso);
        if (usuarioExitoso != null)
        {
            yield return new TestCaseData(usuarioExitoso).SetName("QA_PRN_14_ValidarIdentidadEnDashboard");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerDatosDashboard))]
    public async Task QA_PRN_14_ValidarIdentidadEnDashboard(UsuarioTestData datos)
    {
        LogWriter("Validando la correcta inyección de la identidad del usuario en la sesión activa.");
        await Expect(Page.Locator("body")).ToContainTextAsync(datos.AgenteEsperado);
    }

    [Test]
    public async Task QA_PRN_16_ValidarTarjetasDeEstado()
    {
        LogWriter("Iniciando validación determinista de tarjetas de estado interactivo.");

        var tarjetas = new Dictionary<string, string>
        {
            { "#btnComercialNo", "No Elaborado" },
            { "#btnComercialPendiente", "Pendiente" }
        };

        foreach (var tarjeta in tarjetas)
        {
            LogWriter($"Despachando evento Click sobre tarjeta: {tarjeta.Value}");
            await ClickConMonitoreo(Page.Locator(tarjeta.Key), $"Tarjeta {tarjeta.Value}");
            
            await Expect(Page.Locator("#pptoComercial")).ToBeVisibleAsync();
            
            // 🚨 Sincronización Explícita de Estado (No más esperas ciegas)
            await Page.Keyboard.PressAsync("Escape");
            await Page.Locator(".modal-sn").WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        }
        
        LogWriter("Ciclo de tarjetas de estado completado sin bloqueos de interfaz.");
    }
}
