namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA MÉTRICAS (KPIs)
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_PRN_KpisData.json
// Consumido por: Tests/Pagina Principal/QA_PRN_NavegacionKpisTests.cs
// ---------------------------------------------------------
// Descripción: Define la estructura para iterar sobre los botones 
// circulares del Dashboard y validar la navegación hacia sus detalles.
public class KpiTestData
{
    public string? CasoId { get; set; }
    public string? Boton { get; set; }
    public string? TituloEsperado { get; set; }
    public bool EsId { get; set; }
}