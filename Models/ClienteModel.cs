namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: ENTIDAD CLIENTE PARA GESTIÓN
// ---------------------------------------------------------
// Descripción: Estructura híbrida que soporta tanto Persona 
// Física como Moral, además de almacenar el ID generado por la DB.
//usado en cliente nuevo unicamente
public class ClienteModel
{
    public bool EsPersonaMoral { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string CURP { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string EmailFacturacion { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;
    
    // Dirección
    public string Estado { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumExt { get; set; } = string.Empty;
    public string CP { get; set; } = string.Empty;
    public string ColoniaValue { get; set; } = string.Empty;

    // Output
    public string? IdClienteGenerado { get; set; }
}