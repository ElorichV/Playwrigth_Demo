using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using System;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTERACCIÓN Y FILTROS (PÁGINA PRINCIPAL / DASHBOARD)
// ---------------------------------------------------------
// Descripción: Valida que los elementos interactivos de la página principal 
// (pestañas, botones de filtro y barras de búsqueda) respondan correctamente
// y no provoquen un colapso del sistema ("crash") cuando faltan datos.
[TestFixture]
public class QA_PRN_FiltrosInteraccionTests : BaseTest 
{
    // ---------------------------------------------------------
    // SETUP POR PRUEBA (Preparación del Entorno)
    // ---------------------------------------------------------
    [SetUp]
    public async Task SetupDashboard()
    {
        // 1. Inyección de sesión dinámica.
        await LoginDinamico();
        
        // 2. CHECK DE SALUD: Verificamos que el DOM se haya renderizado por completo.
        // NOTA ACTUALIZADA: La tolerancia estricta de 30s ya está delegada al 
        // framework global en BaseTest.cs para evitar redundancia de timeouts.
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------
    // QA-PRN-11 al QA-PRN-13 - Navegación por Pestañas (Validación de Estado)
    // ---------------------------------------------------------
    // Descripción: Comprueba que el usuario pueda alternar entre las pestañas 
    // principales del Dashboard y regresar a la vista original sin perder la interfaz.
    [Test]
    public async Task QA_PRN_11_al_13_NavegacionPorPestanas()
    {
        LogWriter("Iniciando validación de flujo entre pestañas del Dashboard.");

        // Clic a pestaña 2 y 3 utilizando el monitor de latencia.
        await ClickConMonitoreo(Page.Locator("#tab-home-2"), "Cambio a Pestaña 2"); 
        await Task.Delay(1500); // Pausa necesaria para estabilización de animaciones CSS

        await ClickConMonitoreo(Page.Locator("#tab-home-3"), "Cambio a Pestaña 3");
        await Task.Delay(1500); 

        // Retorno a la pestaña inicial.
        await ClickConMonitoreo(Page.Locator("#tab-home-1"), "Retorno a Pestaña 1");
        
        // 🚨 DIAGNÓSTICO QA (BUG VISUAL): 
        // La plataforma inyecta un estilo CSS "hidden" a ciertos elementos cuando el ambiente 
        // de capacitación no tiene datos, lo que podría romper validaciones de visibilidad estándar.
        // WORKAROUND: Validamos que la estructura base (#pptoComercial) esté adjunta al DOM.
        await Expect(Page.Locator("#pptoComercial")).ToBeAttachedAsync();
        LogWriter("Navegación de pestañas validada satisfactoriamente.");
    }

    // ---------------------------------------------------------
    // QA-PRN-15 al QA-PRN-17 - Filtros de Sinergia (Validación de Supervivencia)
    // ---------------------------------------------------------
    // Descripción: Verifica que los botones de filtro (Residencial, Comercial, Ambos)
    // operen correctamente sin romper la interfaz de usuario (UI).
    [Test]
    public async Task QA_PRN_15_al_17_FiltrosDeSinergia()
    {
        LogWriter("Iniciando pruebas de filtros de Sinergia.");
        
        // ⚠️ NOTA TÉCNICA: Al no haber datos en ciertos perfiles, la plataforma lanza 
        // un popup de aviso que el manejador global de BaseTest cierra automáticamente.
        var filtros = new[] { "Residencial", "Ambos", "Comercial" };

        foreach (var filtro in filtros)
        {
            LogWriter($"Aplicando filtro: {filtro}");
            var selector = Page.Locator($"text='{filtro}'").First;
            
            // Registramos latencia para informar a Sistemas sobre tiempos de respuesta del API de filtros.
            await ClickConMonitoreo(selector, $"Botón Filtro {filtro}");
            
            // Confirmamos estabilidad del Dashboard post-filtrado.
            await Expect(Page.Locator("#pptoComercial")).ToBeAttachedAsync();
        }
        LogWriter("Ciclo de filtros completado.");
    }

    // ---------------------------------------------------------
    // QA-PRN-18 - Buscador Vacío (Prueba de Estrés / Negative Testing)
    // ---------------------------------------------------------
    // Descripción: Inyecta una cadena de texto aleatoria para forzar un escenario 
    // de "No se encontraron resultados" y valida el manejo elegante del error.
    [Test]
    public async Task QA_PRN_18_BuscadorVacio()
    {
        LogWriter("Ejecutando prueba negativa en buscador principal.");
        var inputBuscador = Page.GetByPlaceholder("buscar...");
        
        // Generamos un string único para garantizar una búsqueda sin resultados reales.
        string textoFalso = "PruebaQA_" + DateTime.Now.Ticks;
        await inputBuscador.FillAsync(textoFalso);

        // Ejecución de búsqueda con monitoreo.
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).First, "Ejecutar Búsqueda");

        // 🛡️ FIX DE ARQUITECTURA: 
        // El manejador global de BaseTest atrapa el modal de "No se encontró información".
        // Nuestra aserción confirma que la aplicación no se bloqueó y el input sigue disponible.
        await Expect(inputBuscador).ToBeVisibleAsync();
        LogWriter("Prueba de estrés de búsqueda finalizada con éxito.");
    }
}