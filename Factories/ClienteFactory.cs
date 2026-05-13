using Bogus;
using System.Text.Json;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

public static class ClienteFactory
{
    public static ClienteModel GenerarCliente(string estadoDestino, bool personaMoral = false)
    {
        var f = new Faker("es_MX");
        var direccion = ObtenerDireccionCapital(estadoDestino);
        
        // 🔒 Carga de datos fijos y seguros desde JSON
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "ClienteFijo.json");
        var jsonContent = File.ReadAllText(rutaJson);
        using JsonDocument doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        return new ClienteModel
        {
            EsPersonaMoral = personaMoral,
            
            // --- DATOS SENSIBLES Y DE CONTACTO (ESTRICTAMENTE CONTROLADOS) ---
            RFC = personaMoral ? root.GetProperty("RfcMoralFijo").GetString()! : root.GetProperty("RfcFijo").GetString()!,
            CURP = personaMoral ? "" : root.GetProperty("CurpFija").GetString()!,
            EmailFacturacion = root.GetProperty("EmailFacturacion").GetString()!,
            EmailContacto = root.GetProperty("EmailContacto").GetString()!,
            Telefono = root.GetProperty("TelefonoFijo").GetString()!,
            Celular = root.GetProperty("CelularFijo").GetString()!,
            Contacto = "AXEL ALBERTO LINARES QA", // Fijo para identificar fácilmente tus registros

            // --- DATOS DINÁMICOS INOFENSIVOS (Solo para evitar duplicados en DB) ---
            NombreComercial = $"QA_TEST_{f.Company.CompanyName().ToUpper()}",
            RazonSocial = personaMoral ? $"SA DE CV QA {f.Random.Number(100,999)}" : "FISICA TEST",

            // --- DIRECCIÓN (Controlada por el Factory) ---
            Estado = estadoDestino,
            Ciudad = direccion.Ciudad,
            Calle = direccion.Calle,
            NumExt = direccion.Num,
            CP = direccion.CP,
            ColoniaValue = direccion.ColoniaVal
        };
    }

    private static (string Ciudad, string Calle, string Num, string CP, string ColoniaVal) ObtenerDireccionCapital(string estado)
    {
        return estado switch
        {
            "Jalisco" => ("Guadalajara", "Av. Juárez", "638", "44100", "28932|CENTRO"),
            "Ciudad de México" => ("Venustiano Carranza", "Africa", "78", "15400", "14783|ROMERO RUBIO"),
            "Nuevo León" => ("Monterrey", "Calle de Morelos", "101", "64000", "1|CENTRO"),
            "Puebla" => ("Puebla", "Av. Don Juan de Palafox", "14", "72000", "1|CENTRO"),
            "Guanajuato" => ("Guanajuato", "Plaza de la Paz", "7", "36000", "1|CENTRO"),
            _ => ("Cuauhtémoc", "Paseo de la Reforma", "222", "06600", "1|CENTRO")
        };
    }
}