using System.Collections.Generic;

namespace Playwrigt_Demo.Models;

public class CotizacionTestData
{
    public string CasoId { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Suscripcion { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public List<string> Leyendas { get; set; } = new();
}