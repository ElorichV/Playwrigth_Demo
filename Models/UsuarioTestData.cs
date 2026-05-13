namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA AUTENTICACIÓN E IDENTIDAD
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_LGN_Data.json
// Consumido por: 
//   - Tests/Autenticacion/QA_LGN_AutenticacionTests.cs
//   - Tests/Pagina Principal/QA_PRN_IntegridadDashboardTests.cs
// ---------------------------------------------------------
// Descripción: Estructura para validar flujos de login (éxito/error) 
// y verificar que la identidad del Agente en el Dashboard sea correcta.
public class UsuarioTestData
{
    public string CasoId { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool DebeSerExitoso { get; set; }
    public string MensajeEsperado { get; set; } = string.Empty;
    public string AgenteEsperado { get; set; } = string.Empty;
}