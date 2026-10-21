    # Problemas conocidos del ambiente (no son bugs de la suite de pruebas)

Este documento reúne los defectos de la aplicación PinBox detectados por la suite E2E que
**no se corrigen cambiando el código de pruebas** — o bien porque dependen de un fix en el
backend/frontend de la aplicación, o porque su causa exacta todavía no está confirmada y
requiere revisión manual antes de tocar nada.

Si una prueba falla y el síntoma coincide con algo de aquí, no asumas que el selector está
desactualizado: confírmalo contra este documento primero.

Última actualización: a partir de las corridas del 2026-06-30, 2026-07-02, 2026-07-06 y
2026-07-07 contra `capacitacionpinbox.seccionamarilla.com`. Cada hallazgo abierto trae ahora una
sección **"Opciones de solución"** con pasos concretos para quien continúe este trabajo, no solo
el diagnóstico.

---

## 1. [CRÍTICO] Dashboard del agente responde 500 en cada sesión (`agente_id=0`)

**Estado:** confirmado con evidencia de red, escalado al equipo de backend. Pendiente de
corrección (verificado sin cambios en dos corridas consecutivas al 2026-07-02).

**Síntoma:** justo después de cada login, seis llamadas del dashboard fallan con HTTP 500:

```
GET  /Agente/getDashboardCarteraInicial?agente_id=0
GET  /Agente/getDashboardT?agente_id=0
GET  /Agente/diasVenta?
GET  /Agente/getResumenPerformance?agente_id=0
GET  /Agente/getResumenPubDigMensualSup?agente_id=0
POST /Agente/getDashCategoriaAgente
```

El cuerpo de la respuesta trae esta traza de servidor:

```
Exception of type 'PB_TRY_CATCH.PB_Exception' was thrown.
VentasMobile.Controllers.AgenteController.getDashboardCarteraInicial(String agente_id) +5083
```

**Causa aparente:** el front-end pide estos 6 widgets con `agente_id=0` en vez del ID real del
agente recién autenticado. El controlador no maneja ese caso y lanza una excepción sin capturar.

**Por qué importa para la suite:** este error es, con alta probabilidad, el que dispara las
alertas SweetAlert que `BaseTest` maneja globalmente (`AlertaDashboardApiLenta` y
`AlertaSinInformacion`, ver `Core/BaseTest.cs`). Mientras el bug siga presente, **cualquier
prueba puede fallar de forma intermitente** por una de esas alertas interceptando un clic —
eso no es necesariamente un bug de la prueba ni del framework de automatización.

**Reproducción:** se confirmó en 5 sesiones distintas, con cuentas de agente distintas, en
módulos distintos (Cotizaciones, Login, Smoke, Clientes) y en corridas separadas. Evidencia de
red (`.har`) disponible en `Reportes/Network/<Módulo>/<NombrePrueba>_trafico.har` — cualquier
sesión reciente la reproduce.

**Actualización 2026-07-06:** al correr por primera vez las 26 pantallas nuevas del menú Gestión
(`Category=Gestion`), 7 de 10 fallas mostraron el mismo patrón exacto: el `AlertaSinInformacion`
(`.swal2-container` con texto "no contiene información") interceptando el clic en "Open Menu"
justo después del login. Consistente con este hallazgo, no un bug nuevo — se dejaron los tests
tal cual (selectores correctos) en vez de agregarles manejo especial, ya que el propio proyecto
ya maneja estas alertas globalmente en `SetupGlobal` y el problema de fondo es este bug de
ambiente, no las pruebas.

**Actualización 2026-07-07 (severidad escalada):** en la primera corrida completa de la suite
(140 pruebas, 1h 32m) tras agregar las 26 pantallas de Gestión, **98 de 140 pruebas fallaron**
(25% de aprobación). Del análisis del reporte, la enorme mayoría de esas fallas —incluyendo el
Cotizador completo (69/69 casos data-driven) y Alta de Clientes (2/2)— muestran exactamente este
mismo patrón: el `.swal2-container` de "no contiene información" (o "seguir navegando en
Pinbox") interceptando el login (`LoginDinamico`/`LoginEspecifico`) antes de que la prueba pueda
continuar. Esto es consistente con el hallazgo original, pero mucho más severo que lo reportado
el 2026-07-02 ("puede fallar de forma intermitente") — en esta corrida el bug se disparó en la
mayoría de las sesiones, no en una minoría. Posible causa: la corrida larga y sostenida (92
pruebas originales + 26 nuevas, ~1.5h) pudo agravar la carga sobre el backend del agente. Se
recomienda re-correr en un horario de menor carga y/o por lotes más pequeños para un reporte con
una tasa de aprobación más representativa — un re-intento acotado de 9 pruebas afectadas por este
mismo bug ya mostró que 4/9 pasan limpio al reintentarlas sin cambiar nada (ver sección de
Gestión), confirmando que es intermitencia de ambiente y no una regresión del código de pruebas.

**Opciones de solución:**

1. **Fix de raíz (backend, recomendado):** hacer que el front-end envíe el `agente_id` real del
   agente recién autenticado en las 6 llamadas listadas arriba, en vez de `0`. Es la única opción
   que elimina el problema en vez de mitigarlo — mientras no se aplique, cualquier prueba de la
   suite puede fallar sin importar qué tan bien esté escrita.
2. **Fix alternativo (backend):** que `AgenteController` maneje `agente_id=0` devolviendo una
   respuesta vacía/controlada en vez de lanzar una excepción sin capturar (`PB_Exception`).
   Reduciría el impacto aunque no corrija la causa (el front seguiría pidiendo el ID equivocado).
3. **Mitigación del lado de la suite (mientras no se corrija arriba):** correr la suite completa
   en lotes más pequeños (por módulo, con los filtros de `Category`/`Name` ya documentados en el
   README) en vez de una corrida de 140 pruebas de un tirón — reduce la carga sostenida sobre el
   backend y, según lo observado, mejora la tasa de aprobación real.
4. **Explícitamente descartado:** NO agregar esperas o bloqueos manuales de modal dentro de
   `LoginDinamico`/`LoginEspecifico` para "asegurar" que la alerta se cierre — ya causó una ronda
   de fallos masivos por timeout en el pasado (ver el comentario del propio método en
   `Core/BaseTest.cs`) y es exactamente el anti-patrón que este proyecto ya decidió no repetir.

---

## 2. [ALTA] Alta de Clientes no bloquea el envío de un formulario vacío

**Estado:** confianza media, pendiente de confirmación manual.

**Prueba:** `QA_CLN_01_ValidacionCamposObligatorios` en
`Tests/Menu Lateral/ML_Gestion/ML_Gestion_ClientesNuevos/QA_GST_ClientesTests.cs`.

**Síntoma:** al enviar el formulario de "Cliente nuevo" completamente vacío, no aparece ningún
elemento `.alert, .text-danger, .error-message, .error` dentro de 5 segundos.

**Hipótesis:** si la validación ahora se muestra como un SweetAlert genérico en vez de un texto
en pantalla, el manejador global de alertas (pensado para cerrar avisos de "API lenta") podría
estar cerrando el aviso de validación real antes de que la prueba lo vea. Falta confirmar
manualmente si aparece algún mensaje (de cualquier tipo) al enviar el formulario vacío.

**Impacto si se confirma como bug real:** un agente podría crear un cliente incompleto en el CRM.

**Opciones de solución:**

1. **Primero, confirmar manualmente** (fuera de la suite) qué aparece realmente al enviar el
   formulario vacío: ¿nada?, ¿un SweetAlert genérico?, ¿un mensaje que la prueba no está mirando?
   Sin este paso, cualquier cambio de código sería adivinar.
2. **Si aparece un SweetAlert real de validación:** quitarle el handler global a esta prueba
   puntual con `Page.RemoveLocatorHandlerAsync(...)` al inicio del test (patrón ya usado en el
   proyecto para casos donde se necesita *ver* una alerta en vez de que se auto-cierre), y
   cambiar la aserción de `.alert, .text-danger, .error-message, .error` a `.swal2-container`.
3. **Si de verdad no aparece ningún mensaje:** es un bug real de validación de negocio en el
   frontend/backend de Alta de Clientes — reportarlo para que se agregue una validación visible
   antes de permitir el envío del formulario vacío.

---

## 3. [MEDIA] Validador de "Valida dominio" tiene reglas de negocio invertidas

**Estado:** confirmado, reproducido de forma directa y determinista (sin alertas ni tiempos de
espera de por medio).

**Prueba:** `QA_VDM_ValidarReglasDeNegocioDominios` en
`Tests/Menu Lateral/ML_Dashboard/ML_Dashboard_ValidaDominio/QA_VDM_ValidaDominioTests.cs`.

**Síntoma — regla rota #1:** el dominio `https://axelinares.com.mx` es **aceptado**, cuando el
propio texto de ayuda del formulario dice "Por favor utilice puntos en su dominio. Evite http y
caracteres especiales".

**Síntoma — regla rota #2:** el dominio `axelinares.com` (terminación `.com` común) es
**rechazado** con "La terminación del dominio no es valida" — el mismo mensaje que debería
aparecer solo para terminaciones no admitidas (ej. `facebook.com`, que sí se rechaza
correctamente).

**Lectura:** el motor de validación de dominio funciona, pero su lista/regex de reglas parece
estar desalineada — probablemente en la misma función, ya que ambos síntomas conviven.

**Impacto:** un agente podría registrar un dominio mal formado (con protocolo) para un Sitio
ADN, o que se le rechace sin motivo un dominio `.com` legítimo.

**Opciones de solución:**

1. **Fix de raíz (backend/frontend):** revisar la función de validación de dominio (probablemente
   una regex o una lista de reglas evaluada en el orden equivocado) y corregir ambas reglas:
   rechazar cualquier dominio que contenga `http`/`https`, y aceptar terminaciones `.com`/`.com.mx`
   comunes salvo que estén en una lista explícita de dominios reservados/no admitidos.
2. **Mientras no se corrija:** no "arreglar" la prueba invirtiendo sus aserciones para que pase —
   eso ocultaría el bug real. La prueba (`QA_VDM_ValidarReglasDeNegocioDominios`, con sus 4
   `[TestCase]`) debe seguir documentando el comportamiento actual tal cual es.
3. **Para acelerar el diagnóstico del lado de negocio:** compartir con el equipo de producto la
   lista completa de terminaciones que sí deberían aceptarse (hoy solo se sabe que `.com` común
   se rechaza incorrectamente) para poder ampliar los casos de prueba una vez que se corrija.

---

## 4. [BAJA] Enlace "IAM" ausente en Herramientas → Red Interna

**Estado:** confianza media — puede ser un renombre intencional, no una caída.

**Prueba:** `QA_AYD_ValidarVisibilidadEnlacesHerramientas` en
`Tests/Menu Lateral/ML_Ayuda/ML_Ayudas_Herramientas/QA_AYD_HerramientasTests.cs`.

**Síntoma:** ningún elemento con texto "IAM" existe en el DOM de la pestaña "Red Interna" tras
45 segundos de espera (no está oculto: no se encuentra en absoluto).

**Siguiente paso:** confirmar manualmente si el enlace cambió de nombre o fue removido a
propósito. Si cambió de nombre, es un ajuste de una línea en la prueba, no un bug de producto.

**Opciones de solución:**

1. **Confirmar manualmente primero** entrando a Herramientas → Red Interna con una cuenta real y
   revisando si "IAM" existe con otro nombre, otro ícono, o si de plano ya no está en el menú.
2. **Si cambió de nombre:** actualizar el `Name` del `GetByRole(AriaRole.Link, ...)` en
   `QA_AYD_ValidarVisibilidadEnlacesHerramientas` — cambio de una línea, no requiere tocar nada
   más.
3. **Si fue removido intencionalmente:** quitar esa aserción específica de la prueba (o todo el
   caso si "IAM" ya no aplica) y anotar en el propio test por qué ya no se valida.
4. **Si nadie sabe la respuesta:** preguntar al equipo de producto/negocio de Herramientas antes
   de tocar el código — no asumir ninguna de las dos opciones anteriores sin confirmación.

---

## 5. [ALTA] Comisiones por resultados no muestra tarjetas para diciembre

**Estado:** confianza alta — reproducido idéntico en dos corridas separadas con distinto
Token/RequestId de sesión.

**Prueba:** `QA_CMS_IntegridadDatosTodasLasComisiones` en
`Tests/Menu Lateral/ML_Dashboard/ML_Dashboard_ComisionesResultados/QA_CMS_ComisionesResultadoTests.cs`.

**Síntoma:** al seleccionar el periodo 12 (diciembre), la navegación a
`/agente/bonospagados?periodo=12&anio=<año>` ocurre correctamente, pero ninguna tarjeta de
comisión llega a renderizarse — tampoco aparece un mensaje de "sin datos para este periodo".

**Dos hipótesis, ambas requieren confirmación:**
- La cuenta de prueba no tiene comisiones cargadas para diciembre de ese año, y a la pantalla le
  falta un estado vacío explícito (bug de UX, no de datos).
- La cuenta sí tiene comisiones y la pantalla simplemente no las renderiza (bug de
  renderizado).

**Impacto:** si ocurre en producción con datos reales, un agente no podría verificar sus
comisiones de un periodo — probable generador de tickets de soporte.

**Opciones de solución:**

1. **Confirmar con datos primero:** revisar directamente en la base de datos (HeidiSQL, ver
   `CLAUDE.md`) o con el equipo de backend si la cuenta de prueba usada tiene comisiones
   registradas para diciembre del año consultado. Sin esto, no se puede saber cuál de las dos
   hipótesis aplica.
2. **Si la cuenta no tiene datos para diciembre (bug de UX):** agregar un estado vacío explícito
   en la pantalla (ej. "No hay comisiones registradas para este periodo") — reportar a frontend.
3. **Si la cuenta sí tiene datos (bug de renderizado):** revisar el HAR de la prueba
   (`Reportes/Network/Dashboard/QA_CMS_IntegridadDatosTodasLasComisiones_trafico.har`) para ver
   si la respuesta de red trae los datos y el front simplemente no los pinta, o si la API ya
   responde vacía para ese periodo — dirige el bug a frontend o a backend respectivamente.

---

## 6. [ALTA] Enlace "Envío de Material" redirige a una página de error

**Estado:** confirmado, reproducido de forma directa y determinista (sin alertas ni tiempos de
espera de por medio), durante el mapeo del menú de Gestión con `codegen`.

**Prueba:** `QA_ENV_01_AccesoAEnvioDeMaterial` (en `[Ignore]`) en
`Tests/Menu Lateral/ML_Gestion/ML_Gestion_EnvioMaterial/QA_ENV_EnvioMaterialTests.cs`.

**Síntoma:** al hacer clic en el enlace "Envío de Material" del menú Gestión, la aplicación
redirige a `http://propuestas.i.seccionamarilla.com/error.aspx` en vez de cargar la pantalla
esperada.

**Impacto:** un agente no puede acceder en absoluto a esta funcionalidad del menú.

**Actualización 2026-07-06:** se confirmó el mismo síntoma exacto (misma URL de error) en el
enlace "Cambios a Digitales" — ver hallazgo 8.

**Actualización 2026-07-07:** la corrida completa de la suite confirmó un tercer enlace con el
mismo síntoma: "Solicitud de Propuestas" (`QA_SOP_01_AccesoASolicitudDePropuestas`, en `[Ignore]`,
`Tests/Menu Lateral/ML_Gestion/ML_Gestion_SolicitudPropuestas/QA_SOP_SolicitudPropuestasTests.cs`).
Los tres enlaces del menú Gestión probablemente comparten una causa raíz (ej. una redirección mal
configurada hacia `propuestas.i.seccionamarilla.com`).

**Opciones de solución:**

1. **Confirmar el estado del subdominio:** verificar con el equipo de infraestructura si
   `propuestas.i.seccionamarilla.com` sigue activo. Si fue decomisionado o migrado, es la causa
   raíz de los 3 enlaces y explica por qué comparten exactamente la misma URL de error.
2. **Si el subdominio sigue vivo pero mal enrutado:** revisar la configuración de redirección de
   estos 3 enlaces específicos del menú Gestión (Envío de Material, Cambios a Digitales,
   Solicitud de Propuestas) — probablemente apuntan a una ruta que cambió o nunca se actualizó.
3. **Si el subdominio fue dado de baja intencionalmente:** esos 3 enlaces deberían removerse del
   menú lateral de Gestión en vez de quedar visibles y rotos — reportar a producto para decidir
   si se ocultan o se re-apuntan a un reemplazo.
4. **Mientras no se corrija:** dejar las 3 pruebas (`QA_ENV`, `QA_CAD`, `QA_SOP`) en `[Ignore]`
   tal como están — su aserción ya está escrita a propósito contra el bug (verifica la URL de
   error), así que en cuanto se corrija cualquiera de los tres, esa prueba puntual empezará a
   fallar sola, señal de quitarle el `[Ignore]` y escribir su aserción real.

---

## 7. [ALTA] "Sinergia histórico" no carga contenido — muestra error genérico

**Estado:** confianza alta, reproducido en dos visitas separadas a la pantalla dentro de la misma
sesión de mapeo con `codegen`.

**Prueba:** `QA_SGH_01_AccesoASinergiaHistorico` (en `[Ignore]`) en
`Tests/Menu Lateral/ML_Gestion/ML_Gestion_SinergiaHistorico/QA_SGH_SinergiaHistoricoTests.cs`.

**Síntoma:** al hacer clic en el enlace "Sinergia histórico" del menú Gestión, en vez de cargar
el contenido esperado aparece un diálogo: "No se puede mostrar información... Intente nuevamente
por favor".

**Impacto:** un agente no puede consultar el histórico de Sinergia.

**Opciones de solución:**

1. **Revisar el HAR de la prueba** (`Reportes/Network/Gestion/QA_SGH_01_AccesoASinergiaHistorico_trafico.har`)
   para ver si la llamada de red detrás de "Sinergia histórico" también responde 500 o similar —
   si es así, es plausible que comparta familia con el bug crítico #1 (falta de `agente_id` real).
2. **Si es un endpoint distinto fallando:** escalar como un hallazgo aparte a backend, con la
   traza de red como evidencia (el mismo patrón de diagnóstico que ya funcionó para el hallazgo 1).
3. **Mientras no se corrija:** mantener `QA_SGH_01_AccesoASinergiaHistorico` en `[Ignore]` con su
   aserción actual (verifica que aparezca el diálogo de error) — en cuanto se corrija, la prueba
   empezará a fallar sola, señal de quitarle el `[Ignore]` y escribir la aserción real del
   contenido histórico.

---

## 8. [ALTA] Enlace "Cambios a Digitales" redirige a una página de error

**Estado:** confirmado, reproducido de forma directa y determinista en una corrida real de
`dotnet test` (no solo en la grabación de codegen).

**Prueba:** `QA_CAD_01_AccesoACambiosDigitales` (en `[Ignore]`) en
`Tests/Menu Lateral/ML_Gestion/ML_Gestion_CambiosDigitales/QA_CAD_CambiosDigitalesTests.cs`.

**Síntoma:** al hacer clic en el enlace "Cambios a Digitales" del menú Gestión, la aplicación
redirige a `http://propuestas.i.seccionamarilla.com/error.aspx` — la misma URL de error exacta
que "Envío de Material" (hallazgo 6).

**Impacto:** un agente no puede acceder en absoluto a esta funcionalidad del menú.

**Opciones de solución:** ver hallazgo 6 (Envío de Material) — los tres enlaces con este mismo
síntoma comparten las mismas opciones de solución propuestas ahí.

---

## 9. [Pre-existente] Bugs ya marcados con `[Ignore]` en el código

Estos ya estaban documentados en el código antes de la última ronda de revisión; se listan aquí
para tener un solo lugar de referencia.

| Prueba | Archivo | Síntoma | Opciones de solución |
|---|---|---|---|
| `QA_LGN_ValidarFlujoDeLogin` (caso `QA-LGN-02`) | `Tests/Autenticacion/QA_LGN_AutenticacionTests.cs` | El ambiente permite iniciar sesión con una contraseña incorrecta (`tito.martinez` / `ClaveFalsaSuperMala123`). Referenciado como Ticket #1234. **Bug de seguridad**, prioridad alta si sigue sin resolverse. | Escalar como prioridad de seguridad (no solo de QA) al equipo de backend de autenticación; verificar la lógica de comparación de contraseña/hash. Confirmar el estado del Ticket #1234 antes de re-investigar desde cero. |
| `QA_SNG_ValidacionBonoSinergia` | `Tests/Menu Lateral/ML_Dashboard/ML_Dashboard_BonoSinergia/QA_SNG_BonoSinergia_Tests.cs` | Error 500: "The view 'bonosinergia' was not found". | Es un error de despliegue/configuración (vista faltante en el servidor), no de lógica — verificar con backend que la vista `bonosinergia` exista y esté desplegada en el ambiente de capacitación. |
| `QA_TBL_NavegacionCertificacionVentas` | `Tests/Menu Lateral/ML_Dashboard/ML_Dashboard_Tablero/QA_TBL_TableroTests.cs` | El botón "Ir a Certificación de Ventas" genera un error 500. | Revisar logs de backend del endpoint detrás de ese botón al momento del clic; el HAR de la prueba (`Reportes/Network/Cotizaciones/QA_TBL_NavegacionCertificacionVentas_trafico.har`) es la primera evidencia a mirar. |

---

## Cómo actualizar este documento

Cuando confirmes o descartes un hallazgo (por revisión manual o porque Sistemas ya lo corrigió):

1. Si se corrigió: mueve la entrada a un historial al final de este archivo o bórrala, y si la
   prueba correspondiente tenía un comentario apuntando aquí, actualízalo.
2. Si se confirma como bug real: cambia su estado a "Confirmado" y, si aplica, agrega el número
   de ticket del sistema de seguimiento que se use.
3. Si resulta ser un falso positivo de la prueba (por ejemplo, un cambio de nombre intencional):
   corrige la prueba, no la marques aquí como pendiente.
