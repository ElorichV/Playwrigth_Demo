using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Playwrigt_Demo.Models;

public class ProductoCatalogoModel
{
    [JsonPropertyName("producto")]
    public string Producto { get; set; } = string.Empty;

    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [JsonPropertyName("periodos_validos")]
    public List<string> PeriodosValidos { get; set; } = new();

    [JsonPropertyName("suscripciones_validas")]
    public List<string> SuscripcionesValidas { get; set; } = new();
}