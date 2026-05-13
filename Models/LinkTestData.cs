namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: ESTRUCTURA DE LOS ENLACES DEL MENÚ
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivos JSON: 
//   - TestData/QA_SMK_TableroLinksData.json
//   - TestData/QA_SMK_AyudasLinksData.json
// Consumido por: 
//   - Tests/Menu Lateral/QA_SMK_MenuLateral_Dashboard_Tests.cs
//   - Tests/Menu Lateral/QA_SMK_MenuLateral_Ayudas_Tests.cs
// ---------------------------------------------------------
public class LinkTestData
{
    public string Id { get; set; } = string.Empty;
    public string TextoEnlace { get; set; } = string.Empty;
    public string SelectorValidacion { get; set; } = string.Empty;
    public bool Habilitado { get; set; }
    public bool EsExterno { get; set; }
}