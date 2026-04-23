# 🧪 Plan de Pruebas Automatizadas: Proyecto Pinbox

**Framework:** Playwright + NUnit (C#)
**Arquitectura:** Page Object Model (POM) / Domain-Driven
**Entorno:** Capacitación / QA

---

## 1. Objetivo General
Establecer un framework de pruebas automatizadas escalable, mantenible y robusto para la plataforma Pinbox, asegurando la integridad estructural de la navegación y validando los flujos de negocio críticos mediante simulaciones End-to-End (E2E).

## 2. Enfoque Estratégico y Metodología ("Clean Code & Scalability")

### Metodología: E2E de Caja Negra (Black-Box End-to-End Testing)
Dado que la automatización se realiza sin acceso al código fuente de la plataforma, la estrategia se fundamenta estrictamente en **Pruebas de Caja Negra (Black-Box)**.
* **Simulación Real:** Se utiliza Microsoft Playwright como motor para simular de forma exacta la experiencia y el comportamiento de un usuario real (un vendedor o agente) operando el sistema desde el navegador.
* **Validación Integral (E2E):** El framework valida el flujo completo de la aplicación de principio a fin. Al examinar la funcionalidad exclusivamente desde el exterior y evaluar las respuestas de la interfaz, se asegura de facto que la orquestación de todos los componentes internos (Frontend, Backend, APIs y Base de Datos) funcione correctamente en conjunto.

### Arquitectura de Pruebas (Separation of Concerns)
El proyecto divide el esfuerzo de automatización en dos capas para garantizar el mantenimiento y evitar pruebas frágiles:
* **Pruebas de Humo (Smoke Tests) Estructurales:** Pruebas de navegación rápidas enfocadas en la disponibilidad de la interfaz, menús y ruteo. Garantizan que el sistema "está vivo" y detectan bloqueos de UI (como modales de error por falta de datos).
* **Pruebas Profundas (Deep Dives) Funcionales:** Pruebas dedicadas exclusivamente a la simulación de reglas de negocio en módulos críticos (ej. Cotizador). Ignoran la validación de la navegación general para centrarse en el ingreso de datos, cálculos y resultados esperados.

## 3. Fases de Implementación
* **Fase 1: Core, Autenticación y Dashboard (✅ Completada)**
* **Fase 2: Navegación y Enrutamiento (🔄 En Progreso):** Pruebas de interacción con los controles de la interfaz (Hamburguesas) y validación de enlaces del menú lateral.
* **Fase 3: Deep Dive Funcional - Cotizador (🎯 Siguiente Objetivo):** Automatización exhaustiva del primer módulo crítico de negocio.

## 4. Stack Tecnológico
* **Lenguaje:** C# (.NET 10.0)
* **Framework de Pruebas:** NUnit (Aserciones, Data-Driven Testing y Paralelización).
* **Automatización UI:** Microsoft Playwright.

---

# 📊 Matriz de Ejecución de Pruebas

## 🔐 Módulo 1: Autenticación (Login)
*Validación de reglas de acceso y aislamiento de sesión.*

| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC01** | Login Exitoso (Redirección a Dashboard) | Data-Driven | ✅ |
| **TC02** | Login Denegado por Contraseña Incorrecta | Data-Driven | ✅ |
| **TC03** | Login Denegado por Usuario Inexistente | Data-Driven | ✅ |

---

## 📈 Módulo 2: Integridad del Dashboard
*Validación de carga de datos base sin navegación adicional.*

| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC04** | Identidad del Agente (Cabecera dinámica) | UI | ✅ |
| **TC05** | Carga de KPI Base (Presupuesto, Solicitudes, Alcance) | UI | ✅ |
| **TC06** | Interactividad de Tarjetas de Estado (Círculos Azules) | UI | ✅ |

---

## 🧭 Módulo 3: Navegación Superior
*Validación de redirecciones a través de los enlaces de KPIs.*

| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC07** | Enlaces de KPI Circulares (Clic y Retorno) | Data-Driven | ✅ |

---

## 🎛️ Módulo 4: Filtros e Interacción de UI
*Validaciones asíncronas de la interfaz (Single Page Application).*

| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC08** | Navegación por Pestañas (Dashboard, Gestión, Ayudas) | UI | ✅ |
| **TC09** | Filtros de Sinergia (Comercial, Residencial, Ambos) | UI | ✅ |
| **TC10** | Manejo de Buscador Vacío (Prueba de Cortafuegos) | Seguridad | ✅ |

---

## 🍔 Módulo 5: Enrutamiento General y Contextos (Smoke Tests)
*Validación de la columna vertebral de navegación, apertura de menús y renderizado contextual desde el Portal del Agente.*

### 5.0 Validación de Interfaz y Contexto
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC11** | Apertura y Cierre de Menú Hamburguesa (Comportamiento Base UI) | UI | 🔄 |
| **TC12** | Validación de Cambio de Contexto (El menú cambia según la sección activa) | Navegación | 🔄 |

### 5.1 Enrutamiento: Menú Operativo (Portal del Agente)
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC13** | Redirección: Visor de Plantillas | Navegación | 🔄 |
| **TC14** | Redirección: Cartera Vencida | Navegación | 🔄 |
| **TC15** | Redirección Base al Módulo "Cotizador" | Navegación | 🔄 |

### 5.2 Enrutamiento: Menú de Gestión de Ventas
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC16** | Redirección: Estadísticas de Citas | Navegación | 🔄 |
| **TC17** | Redirección: Prospectos TMX | Navegación | 🔄 |
| **TC18** | Redirección: Creación HTBS | Navegación | 🔄 |

### 5.3 Enrutamiento: Menú de Recursos y Ayuda
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC19** | Redirección: Glosario | Navegación | 🔄 |
| **TC20** | Redirección: Herramientas | Navegación | 🔄 |
| **TC21** | Redirección: Proceso de Ventas | Navegación | 🔄 |

---

## 💼 Módulo 6: Deep Dive - Menú Dashboard
*Pruebas E2E de Caja Negra orientadas exclusivamente a las reglas de negocio de las herramientas de trabajo que aparecen al activar la pestaña "Dashboard".*

### 6.1 Submódulo: Cotizador (🔥 Prioridad 1)
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC22** | Carga inicial del formulario de Cotización | Funcional | ❌ |
| **TC23** | Cotización Básica Exitosa (End-to-End) | E2E | ❌ |
| **TC24** | Validación de campos obligatorios en Cotizador | Funcional | ❌ |

### 6.2 Submódulo: Visor de Plantillas
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC25** | *[Por definir: Carga de UI y validación de filtros]* | Funcional | ❌ |

### 6.3 Submódulo: Casos
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC26** | *[Por definir: Consulta y apertura de un caso existente]* | Funcional | ❌ |

---

## 📊 Módulo 7: Deep Dive - Menú Gestión
*Pruebas funcionales para los flujos de seguimiento y prospección de clientes que aparecen al activar la pestaña "Gestión".*

### 7.1 Submódulo: Entrevista
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC27** | *[Por definir: Carga de UI Entrevista]* | Funcional | ❌ |

### 7.2 Submódulo: Prospectos TMX
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC28** | *[Por definir: Flujo de registro de nuevo prospecto]* | Funcional | ❌ |

---

## 📚 Módulo 8: Deep Dive - Menú Ayudas
*Validación de material de apoyo y herramientas adicionales que aparecen al activar la pestaña "Ayudas".*

### 8.1 Submódulo: Herramientas
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC29** | *[Por definir: Carga de UI y descarga de manuales]* | Funcional | ❌ |

### 8.2 Submódulo: Glosario
| ID | Caso de Prueba | Tipo | Estado |
| :--- | :--- | :---: | :---: |
| **TC30** | *[Por definir: Búsqueda de términos]* | Funcional | ❌ |
---

**Leyenda de Estados:**
* ✅ **Automatizado y Estable**
* 🔄 **En Desarrollo / Pruebas**
* ❌ **Pendiente**