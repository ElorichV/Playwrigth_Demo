namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: CONFIGURACIÓN GLOBAL DEL AMBIENTE
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/Global_Credentials.json
// Consumido por: Core/BaseTest.cs (Método SetupGlobal)
// ---------------------------------------------------------
// Descripción: Define la estructura para las credenciales de 
// acceso base y la URL del entorno de capacitación de Pinbox.
public class ConfigData
{
    public string Url { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}// JAMÁS debe ser subido al repositorio (asegurarse de que esté en el .gitignore).