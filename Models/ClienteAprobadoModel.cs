using System.Text.Json.Serialization;

namespace Playwrigt_Demo.Models;

public class ClienteAprobadoModel
{
    [JsonPropertyName("idCliente")]
    public string IdCliente { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("municipio")]
    public string Municipio { get; set; } = string.Empty;

    [JsonPropertyName("categoria")]
    public string Categoria { get; set; } = string.Empty;
}