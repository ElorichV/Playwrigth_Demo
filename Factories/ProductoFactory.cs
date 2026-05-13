using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

public static class ProductoFactory
{
    private static List<ProductoCatalogoModel> _catalogo = new();

    static ProductoFactory()
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_GST_CatalogoProductos.json");
        
        if (File.Exists(ruta))
        {
            var jsonContent = File.ReadAllText(ruta);
            _catalogo = JsonSerializer.Deserialize<List<ProductoCatalogoModel>>(jsonContent) ?? new();
        }
        else
        {
            throw new Exception($"No se encontró el catálogo de productos en: {ruta}");
        }
    }

    public static ProductoCatalogoModel? ObtenerConfiguracion(string nombreProducto)
    {
        return _catalogo.FirstOrDefault(p => p.Producto == nombreProducto);
    }
}