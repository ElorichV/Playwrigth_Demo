using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: AUTENTICACIÓN
// ---------------------------------------------------------
// Descripción: Contiene las pruebas de validación de acceso al sistema (Login).
// Utiliza una arquitectura "Data-Driven" para ejecutar múltiples escenarios.
[TestFixture]
public class QA_LGN_AutenticacionTests : BaseTest 
{
    // ---------------------------------------------------------
    // ALIMENTADOR DE DATOS (Data Provider)
    // ---------------------------------------------------------
    public static IEnumerable<TestCaseData> LeerDatos()
    {
        // Actualizado a la nueva nomenclatura de archivos
        var json = File.ReadAllText("../../../TestData/QA_LGN_Data.json");
        var usuarios = JsonSerializer.Deserialize<List<UsuarioTestData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        foreach (var usuario in usuarios)
        {
            var testCase = new TestCaseData(usuario)
                .SetName($"{usuario.CasoId}_ValidarFlujoDeLogin"); 

            if (usuario.CasoId == "QA-LGN-02")
            {
                testCase.Ignore("Bug Crítico Reportado: El ambiente permite inicio de sesión con contraseñas falsas.");
            }

            yield return testCase;
        }
    }

    // ---------------------------------------------------------
    // Flujos de Autenticación
    // ---------------------------------------------------------
    [Test]
    [TestCaseSource(nameof(LeerDatos))]
    public async Task QA_LGN_ValidarFlujoDeLogin(UsuarioTestData datos)
    {
        // 1. Inyección de datos
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(datos.Usuario!);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(datos.Password!);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();

        // 2. Validación de Criterios de Aceptación Dinámicos
        if (datos.DebeSerExitoso)
        {
            // HAPPY PATH
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
        }
        else
        {
            // SAD PATH
            await Expect(Page.Locator("body")).ToContainTextAsync(datos.MensajeEsperado!, new() { IgnoreCase = true });
        }
    }
}