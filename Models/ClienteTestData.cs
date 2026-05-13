namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: LECTURA DEL JSON (Reglas de Negocio)
// ---------------------------------------------------------
public class ClienteTestData
{
    public string CasoId { get; set; } = string.Empty;
    public string TipoPersona { get; set; } = string.Empty;
    public string CategoriaValor { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty; // 👈 Ahora viene del JSON
    public string CURP { get; set; } = string.Empty; // 👈 Ahora viene del JSON
}