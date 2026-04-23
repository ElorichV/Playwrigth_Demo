using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
public class LoginTests : BaseTest 
{
    public static IEnumerable<UsuarioTestData> LeerDatos()
    {
        var json = File.ReadAllText("TestData/usuarios.json");
        return JsonSerializer.Deserialize<List<UsuarioTestData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    [Test]
    [TestCaseSource(nameof(LeerDatos))]
    public async Task ValidarFlujoDeLogin(UsuarioTestData datos)
    {
        await Page.GotoAsync("https://capacitacionpinbox.seccionamarilla.com/", new() { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });

        await Page.Context.ClearCookiesAsync();
        await Page.EvaluateAsync("window.localStorage.clear();");
        await Page.EvaluateAsync("window.sessionStorage.clear();");
        await Page.ReloadAsync(); 

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(datos.Usuario!);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(datos.Password!);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();

        if (datos.DebeSerExitoso)
        {
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync(new() { Timeout = 30000 });
        }
        else
        {
            await Expect(Page.Locator("body")).ToContainTextAsync(datos.MensajeEsperado!, new() { IgnoreCase = true, Timeout = 15000 });
        }
        
        await Task.Delay(1000); 
    }
}