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
* **Simulación Real:** Se utiliza Microsoft Playwright como motor para simular de forma exacta la experiencia y el comportamiento de un usuario real operando el sistema desde el navegador.
* **Validación Integral (E2E):** El framework valida el flujo completo de la aplicación de principio a fin. Al examinar la funcionalidad desde el exterior y evaluar la interfaz, se asegura que la orquestación de todos los componentes internos funcione correctamente.

### Arquitectura de Pruebas (Separation of Concerns)
El proyecto divide el esfuerzo de automatización en dos capas:
* **Pruebas de Humo (Smoke Tests) Estructurales:** Pruebas de navegación rápidas enfocadas en la disponibilidad de la interfaz, menús y ruteo. Detectan errores 500 o bloqueos de UI rápidamente.
* **Pruebas Profundas (Deep Dives) Funcionales:** Pruebas dedicadas exclusivamente a la simulación de reglas de negocio en módulos críticos (ej. Cotizador). Se centran en el ingreso de datos, cálculos y resultados.

## 3. Fases de Implementación
* **Fase 1: Core, Autenticación y Página Principal (✅ Completada)**
* **Fase 2: Navegación y Enrutamiento (🔄 En Progreso):** Pruebas de interacción con los controles de la interfaz y validación masiva de enlaces de menús laterales.
* **Fase 3: Deep Dive Funcional - Cotizador y Clientes (🎯 Siguiente Objetivo):** Automatización exhaustiva de los módulos críticos definidos por Sistemas.

## 4. Stack Tecnológico
* **Lenguaje:** C# (.NET 10.0)
* **Framework de Pruebas:** NUnit (Aserciones, Data-Driven Testing).
* **Automatización UI:** Microsoft Playwright.

---

# 📊 Matriz de Ejecución de Pruebas

## 📖 Glosario de Nomenclatura (Prefijos)
Para mantener la escalabilidad, los Casos de Prueba (TC) utilizan un sistema de prefijos basado en las consonantes del módulo:
*   **QA-LGN-** : Login (Autenticación)
*   **QA-PRN-** : Página Principal (Cabecera, KPIs, Pestañas, Filtros)
*   **QA-SMK-** : Smoke Tests (Pruebas barredoras de menús laterales)
*   **QA-CTZ-** : Cotizador
*   **QA-CLN-** : Cliente Nuevo
*   **QA-CNT-** : Contratos
*   **QA-TAB-** : Tablero (Deep Dives)
*   **QA-GST-** : Gestión Operativa (Deep Dives)
*   **QA-YDS-** : Ayudas y Herramientas (Deep Dives)

---

## 🔐 Módulo 1: Login
*Validación de acceso y seguridad.*

| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-LGN-01** | Login Exitoso (Redirección a Página Principal) | ✅ |
| **QA-LGN-02** | Login Denegado (Contraseña Incorrecta) | 🔄 (Bug) |
| **QA-LGN-03** | Login Denegado (Usuario Inexistente) | ✅ |

---
# 🏠 Módulo 2: Página Principal
*Validación de los elementos fijos, interactividad de KPIs y filtros iniciales.*

### Módulo 2.1 al 2.8: Botones KPI (Circulares)
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-01** | Navegación y retorno: Contratos en OLBC | ✅ |
| **QA-PRN-02** | Navegación y retorno: Contratos rechazados | ✅ |
| **QA-PRN-03** | Navegación y retorno: Contratos en revisión | ✅ |
| **QA-PRN-04** | Navegación y retorno: Contratos en ingreso | ✅ |
| **QA-PRN-05** | Navegación y retorno: Estación IC | ✅ |
| **QA-PRN-06** | Navegación y retorno: Contratos en fulfillment | ✅ |
| **QA-PRN-07** | Navegación y retorno: Contratos publicados | ✅ |
| **QA-PRN-08** | Navegación y retorno: Casos | ✅ |
| **QA-PRN-09** | Navegación y retorno: Al día | ✅ |
| **QA-PRN-10** | Navegación y retorno: Cambios y correcciones | ✅ |

### Módulo 2.9: Pestañas de Navegación Central
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-09** | Renderizado de vista al cambiar a "Dashboard" | ✅ |
| **QA-PRN-10** | Renderizado de vista al cambiar a "Gestión" | ✅ |
| **QA-PRN-11** | Renderizado de vista al cambiar a "Ayudas" | ✅ |

### Módulo 2.10: Filtros de Sinergia y Cabecera
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-PRN-12** | Integridad de Cabecera (Identidad del Agente) | ✅ |
| **QA-PRN-13** | Aplicación de filtro: Comercial | ✅ |
| **QA-PRN-14** | Aplicación de filtro: Residencial | ✅ |
| **QA-PRN-15** | Aplicación de filtro: Ambos | ✅ |

---

## 🧭 Módulo 3: Menús Laterales (Pruebas de Navegación / Smoke)
*Confirman que el clic en el enlace lateral abre la pantalla correcta y se puede regresar (Data-Driven).*

### Módulo 3.1: Comportamiento Base
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-01** | Apertura y Cierre de Menú Hamburguesa | ✅ |

### Módulo 3.2: Menú Lateral Tablero
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-02** | Barredor dinámico de enlaces en Tablero (6 enlaces) | ❌ |

### Módulo 3.3: Menú Lateral Gestión
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-03** | Barredor dinámico de enlaces en Gestión (~26 enlaces) | ❌ |

### Módulo 3.4: Menú Lateral Ayudas
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-SMK-04** | Barredor dinámico de enlaces en Ayudas (11 enlaces) | ❌ |

---

## 💼 Módulo 4: Apartado Gestión (Deep Dives Funcionales Core)
*Pruebas End-to-End de los flujos críticos (Prioridad 1 para Sistemas).*

### Módulo 4.1: Cotizador
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CTZ-01** | Carga inicial del formulario y validación de combos | 🔄 |
| **QA-CTZ-02** | Creación completa de cotización exitosa (E2E) | 🔄 |
| **QA-CTZ-03** | Validación de campos obligatorios vacíos | ❌ |

### Módulo 4.2: Cliente Nuevo
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CLN-01** | Carga de formulario, validación de RFC y prevención de duplicados | ❌ |
| **QA-CLN-02** | Alta exitosa de cliente comercial (E2E) | ❌ |

### Módulo 4.3: Contratos Creados
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-CNT-01** | Búsqueda y visualización de detalle de contrato | ❌ |
| **QA-CNT-02** | Descarga/Exportación de reportes | ❌ |

---

## 📈 Módulo 5: Apartado Tablero (Deep Dives Funcionales)
*Pruebas profundas de interacción dentro de las pantallas del menú Tablero.*

### Módulo 5.1: Comisiones por resultados
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-TAB-01** | Filtrado por periodo y visualización de montos | ❌ |

*(Se agregarán sub-módulos progresivamente)*

---

## 📚 Módulo 6: Apartado Ayudas (Deep Dives Funcionales)
*Pruebas de interacción dentro de las pantallas del menú Ayudas.*

### Módulo 6.1: Herramientas y Documentación
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-YDS-01** | Glosario: Búsqueda exacta de términos | ❌ |
| **QA-YDS-02** | Descarga de software y manuales de apoyo | ❌ |

---
**Leyenda de Estados:**
* ✅ **Automatizado y Estable:** Ejecución regular en verde.
* 🔄 **En Desarrollo / Cuarentena:** Construyéndose o pausado por bug del sistema.
* ❌ **Pendiente de Desarrollo:** Mapeado para futuras iteraciones.

---
---

## 📋 PLANTILLA VACÍA PARA NUEVOS MÓDULOS

## [Icono] Módulo X: [Nombre del Módulo]
*[Breve descripción de lo que cubre este módulo]*

### Módulo X.1: [Sub-módulo]
| ID | Caso de Prueba | Estado |
| :--- | :--- | :---: |
| **QA-XXX-01** | [Descripción de la prueba 1] | ❌ |
| **QA-XXX-02** | [Descripción de la prueba 2] | ❌ |