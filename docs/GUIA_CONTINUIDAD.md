# Guía de continuidad — Suite E2E PinBox

Este documento es el punto de entrada para cualquier persona que retome este proyecto sin haber
participado en su construcción. Está escrito asumiendo que quien lo lee **no tiene contexto
previo** — solo el repositorio y este archivo.

Si solo vas a leer un documento antes de tocar código, que sea este. De aquí te remite a los
demás según lo que necesites.

---

## 1. Qué es este proyecto, en 30 segundos

Suite de automatización E2E (Playwright + NUnit + C#) para PinBox
(`capacitacionpinbox.seccionamarilla.com`), un portal de agentes de ventas. Simula a un agente
real: login, cotizaciones, alta de clientes, comisiones, y navegación completa del menú lateral
(Dashboard, Gestión, Ayudas). Es caja negra: no hay acceso al código fuente de PinBox, todo se
valida por comportamiento observable (UI + tráfico de red).

Es un proyecto de portafolio (uso público autorizado) construido durante un servicio social —
ver `CLAUDE.md` para las convenciones exactas que sigue el código.

---

## 2. Mapa de documentos — qué leer según lo que necesites

| Si necesitas... | Lee... |
|---|---|
| Instrucciones y convenciones obligatorias del proyecto | `CLAUDE.md` (raíz) |
| Arquitectura, estructura de carpetas, cómo correr pruebas, cobertura completa | `README.md` (raíz) |
| Bugs de la APLICACIÓN ya detectados (no de las pruebas), con opciones de solución para cada uno | `docs/PROBLEMAS_CONOCIDOS.md` |
| Selectores confirmados de las 26 pantallas de Gestión mapeadas con codegen | `docs/codegen_pantallas_gestion.txt` |
| Comandos de ejecución (por módulo, por categoría, combinados) listos para copiar/pegar | `TestResults/Notas.txt` |
| Esta guía — panorama general y próximos pasos | `docs/GUIA_CONTINUIDAD.md` (este archivo) |

---

## 3. Estado actual (al 2026-07-07)

- **140 pruebas totales** en la suite (92 originales + ~48 agregadas en la sesión del
  2026-07-06/07: 26 pantallas nuevas de Gestión + 4 marcadas `[Ignore]` con bug confirmado dentro
  de ese mismo lote).
- **Última corrida completa:** 36 pasaron, 96 fallaron, 8 en `[Ignore]`, 25% de aprobación,
  1h36m de duración.
- **Ese 25% NO refleja la salud real de la suite.** Casi todas las fallas (confirmado
  analizando el reporte dos veces, en corridas separadas) son un solo bug crítico de ambiente ya
  documentado (`docs/PROBLEMAS_CONOCIDOS.md`, hallazgo 1) disparándose con mucha frecuencia
  durante corridas largas y sostenidas — no defectos de las pruebas. Un re-intento acotado de 9
  pruebas afectadas mostró 4/9 pasando limpio sin cambiar nada, confirmando que es intermitencia
  de ambiente.
- **Módulo Gestión:** ahora 100% mapeado (30 clases, ver README sección 7 y 7.1). De esas, 4
  quedaron en `[Ignore]` por bugs reales confirmados de la aplicación (no de las pruebas):
  Envío de Material, Cambios a Digitales, Solicitud de Propuestas (los 3 redirigen a la misma
  página de error) y Sinergia histórico (diálogo de error).
- **Único módulo pendiente de automatizar por completo:** ninguno a nivel de "no existe la
  carpeta" — todo el menú lateral (Dashboard, Gestión, Ayudas) tiene al menos un esqueleto. Lo que
  queda pendiente es **profundidad**: varias pantallas de Gestión solo tienen un smoke de acceso
  (navega y valida un elemento), no cobertura de reglas de negocio — ver sección 5 de esta guía.

---

## 4. Cómo levantar el entorno desde cero

Ver `README.md` sección 4 para el detalle completo. Resumen:

1. `dotnet restore`
2. Instalar navegadores de Playwright: `pwsh bin/Debug/net10.0/playwright.ps1 install`
3. Copiar los 4 archivos `TestData/*.example.json` a su nombre real y completarlos con
   credenciales válidas del ambiente de capacitación (`Global_Credentials.json`,
   `ClienteFijo.json`, `QA_GST_ClientesAprobados.json`, `QA_LGN_Data.json`).
4. **Antes de hacer público el repo o compartirlo más ampliamente:** los archivos reales (no solo
   los `.example.json`) están versionados con credenciales de capacitación — sacarlos del control
   de versiones (`git rm --cached` + `.gitignore`) y rotar las contraseñas expuestas en el
   historial de git. Esto sigue pendiente al momento de escribir esta guía.

---

## 5. Trabajo pendiente, en orden de prioridad

Esta es la lista concreta para quien continúe. No son ideas — son huecos reales identificados
durante la sesión del 2026-07-06/07.

### Prioridad alta

1. **Confirmar y escalar el bug crítico #1** (`docs/PROBLEMAS_CONOCIDOS.md`) con el equipo de
   backend de PinBox — es la causa de la enorme mayoría de fallas de toda la suite, incluido el
   Cotizador completo. Mientras no se corrija, **cualquier reporte de corrida completa va a verse
   peor de lo que realmente está la suite.**
2. **Sacar las credenciales reales del control de versiones** (ver sección 4) — pendiente de
   seguridad antes de compartir el repo más ampliamente.
3. **Confirmar manualmente los 3 hallazgos con "confianza media/pendiente de confirmación"**
   en `PROBLEMAS_CONOCIDOS.md` (Alta de Clientes sin bloqueo, enlace IAM, Comisiones diciembre) —
   cada uno ya trae una sección "Opciones de solución" con los pasos concretos a seguir.

### Prioridad media

4. **Refinar las 4 pantallas de Gestión con "aserción débil"** documentada en su propio comentario
   de clase: `QA_CLC_ClientesCercanosTests`, `QA_HTB_CreacionHTBSTests` (la grabación de codegen
   no capturó un elemento distintivo — hace falta una sesión de mapeo manual más cuidadosa de
   estas 2 pantallas puntuales; las otras 2 que tenían este problema, Cambios a Digitales y
   Solicitud de Propuestas, resultaron ser bugs reales y ya están resueltas).
5. **Confirmar si `QA_PRQ_ProspectosCalificadosTests` y `QA_PRS_ProspectosTests` son la misma
   pantalla** — ambas mostraron el mismo heading genérico "Prospectos" en la grabación de
   codegen; puede que compartan código y solo cambie un filtro.
6. **Ampliar la cobertura de negocio en las 26 pantallas nuevas de Gestión** — hoy son smoke de
   acceso (navegar + validar que carga), no validan reglas de negocio como sí lo hacen Cotizador
   o Alta de Clientes. Priorizar según lo que más use el negocio (Oportunidades, Casos,
   Prospectos son buenos candidatos por volumen de uso).
7. **Decidir la estrategia de login de `QA_CTR_ContratosCreadosTests`** (hoy en `[Ignore]`) —
   confirmar si `LoginDinamico` siempre trae contratos o si, como Cotizaciones/Clientes, depende
   del cliente/agente logueado.

### Prioridad baja

8. **Revisar el caso aislado de timeout en la pestaña "Tarifas"** de
   `QA_AYD_NavegacionAcordeonVentas` — solo una ocurrencia durante una corrida con estrés
   ambiental severo, no hay evidencia suficiente para documentarlo como bug confirmado todavía.
9. **Agregarle `<summary>` de clase a cualquier prueba nueva** que no siga la convención — al
   2026-07-07, las 30 clases del módulo Gestión ya lo tienen; si se agregan módulos nuevos,
   mantener la misma convención (ver README sección 6, checklist punto 7).

---

## 6. Cómo trabajar con los bugs documentados (flujo recomendado)

Cuando una prueba falle:

1. **No asumas que el selector está desactualizado.** Primero confirma contra
   `docs/PROBLEMAS_CONOCIDOS.md` si el síntoma ya está documentado ahí.
2. Si coincide con el hallazgo 1 (SweetAlert interceptando el login): no es nada que arreglar en
   el código de la prueba — es el bug de ambiente ya conocido. Considera correr en un lote más
   pequeño o en otro horario.
3. Si es un síntoma nuevo: reprodúcelo al menos una vez más antes de documentarlo (para
   distinguir un bug real de una casualidad de red), y agrégalo a `PROBLEMAS_CONOCIDOS.md`
   siguiendo el formato de los hallazgos existentes (Estado / Prueba / Síntoma / Impacto /
   Opciones de solución).
4. Si el bug es real y bloquea la prueba de forma consistente: márcala `[Ignore("motivo")]` con
   una razón que apunte al hallazgo correspondiente, y —cuando aplique— escribe la aserción
   *contra* el bug (ver `QA_ENV_EnvioMaterialTests` como ejemplo) para que la prueba misma avise
   el día que el bug se corrija.

---

## 7. Convenciones que no debes romper (resumen — el detalle completo vive en `CLAUDE.md`)

- Todas las clases de prueba heredan de `BaseTest`.
- Login: `LoginDinamico()` para cuenta genérica, `LoginEspecifico(usuario, password)` cuando cada
  caso necesita una identidad propia (Cotizaciones, Clientes). Nunca ambos en la misma clase.
- Nunca `WaitForLoadStateAsync(NetworkIdle)` — usa `Expect(...).ToBeVisibleAsync()`.
- Nunca agregues bloqueos manuales de modal dentro de `LoginDinamico`/`LoginEspecifico` — ya causó
  fallos masivos por timeout en el pasado.
- Usa `RemoveLocatorHandlerAsync` por handler individual si una prueba necesita *ver* una alerta
  de SweetAlert en vez de que se auto-cierre.
- `GetByText` con `Exact = true` cuando haya ambigüedad por substrings (ej. "Prospectos" vs
  "Prospectos TMX").
- Si agregas un prefijo de prueba nuevo, regístralo en `ExtraerModulo()` (`Core/BaseTest.cs`) para
  que su evidencia caiga en su propia carpeta de `Reportes/` en vez de "Generales".

---

## 8. Preguntas frecuentes para quien retoma esto

**¿Por qué la suite completa da ~25% de aprobación si el README dice que casi todo está
"Automatizado"?** Por el bug crítico #1 (sección 3 de esta guía). "Automatizado" describe que el
flujo está escrito y es correcto, no que la corrida de hoy salga en verde — la propia tabla de
cobertura del README lo advierte explícitamente.

**¿Por qué hay pruebas con la aserción escrita "al revés" (esperando que aparezca un error)?**
Es intencional para las pantallas con un bug de aplicación confirmado (`QA_ENV`, `QA_CAD`,
`QA_SOP`, `QA_SGH`) — así, el día que alguien corrija el bug en producción, esa prueba puntual
empieza a *fallar* sola, que es la señal de quitarle el `[Ignore]` y escribir la aserción real.

**¿Dónde está la evidencia de una corrida (video, HAR, logs)?** En `Reportes/`, organizada por
módulo (no versionado en git). El HAR de red es la primera fuente para diagnosticar cualquier
hallazgo nuevo — ver README sección 1.1.
