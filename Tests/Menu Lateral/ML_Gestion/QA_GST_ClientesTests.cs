using Microsoft.Playwright;
using NUnit.Framework;
using Playwrigt_Demo.Factories;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
public class QA_GST_ClientesTests : BaseTest
{
    [Test]
    public async Task QA_GST_01_CrearClienteNuevo_Hibrido()
    {
        // 1. Preparar datos (Cambiamos a true para Persona Moral si se desea)
        var cliente = ClienteFactory.GenerarCliente("Jalisco", personaMoral: false);
        LogWriter($"Iniciando creación de cliente: {cliente.NombreComercial}");

        // 2. Navegación (Usa los métodos heredados de BaseTest)
        await LoginDinamico();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = " Cliente nuevo" }).ClickAsync();

        // 3. Llenado Dinámico
        if (cliente.EsPersonaMoral) 
            await Page.GetByText("Moral").ClickAsync();
        else 
            await Page.GetByText("Fisica").ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "RFC" }).FillAsync(cliente.RFC);
        if (!cliente.EsPersonaMoral) await Page.GetByRole(AriaRole.Textbox, new() { Name = "CURP" }).FillAsync(cliente.CURP);
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Razón social" }).FillAsync(cliente.RazonSocial);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Nombre comercial" }).FillAsync(cliente.NombreComercial);
        
        await Page.Locator("#domFisCalle").FillAsync(cliente.Calle);
        await Page.Locator("input[name=\"domFisNumExt\"]").FillAsync(cliente.NumExt);
        await Page.Locator("input[name=\"domFisCP\"]").FillAsync(cliente.CP);
        await Page.Locator("#domFisColonia").SelectOptionAsync(new[] { cliente.ColoniaValue });

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contacto", Exact = true }).FillAsync(cliente.Contacto);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Teléfono" }).FillAsync(cliente.Telefono);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Correo para facturas" }).FillAsync(cliente.EmailFacturacion);

        // 4. Creación y Manejo de Pop-up
        await Page.GetByRole(AriaRole.Button, new() { Name = "Crear Cliente" }).ClickAsync();
        
        // Manejador simple para el aviso de 24 horas (SweetAlert/Dialog)
        var popupAviso = Page.GetByText("24 horas"); 
        if (await popupAviso.IsVisibleAsync()) await Page.Keyboard.PressAsync("Escape"); // O click en botón OK

        // 5. Captura de ID de Cliente (Scroll si es necesario)
        var fila = Page.Locator("tr").Filter(new() { HasText = cliente.NombreComercial });
        await fila.ScrollIntoViewIfNeededAsync();
        cliente.IdClienteGenerado = await fila.Locator("td").Nth(1).InnerTextAsync();
        
        LogWriter($"[EXITO] Cliente creado con ID: {cliente.IdClienteGenerado}");

        // 6. Retorno Seguro
        await Page.Locator("#back a").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Clientes" })).ToBeVisibleAsync();
    }
}