using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

public static class ClientePoolFactory
{
    private static List<ClienteAprobadoModel> _pool = new();
    private static readonly Random _random = new();

    static ClientePoolFactory()
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_GST_ClientesAprobados.json");
        
        if (File.Exists(ruta))
        {
            var jsonContent = File.ReadAllText(ruta);
            _pool = JsonSerializer.Deserialize<List<ClienteAprobadoModel>>(jsonContent) ?? new();
        }
        else
        {
            throw new Exception($"No se encontró el pool de clientes en: {ruta}");
        }
    }

    public static ClienteAprobadoModel ObtenerClienteAleatorio()
    {
        if (!_pool.Any()) throw new Exception("El pool de clientes está vacío.");
        return _pool[_random.Next(_pool.Count)];
    }
}