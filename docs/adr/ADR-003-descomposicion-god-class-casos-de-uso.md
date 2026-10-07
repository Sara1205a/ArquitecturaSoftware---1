# ADR-003: Descomposición de la God Class en casos de uso y segregación de interfaces

| Campo | Valor |
|---|---|
| **ID** | ADR-003 |
| **Estado** | Aceptado |
| **Fecha** | 2026-10-06 |
| **Autores** | [Sara Arboleda Uribe] |
| **Revisores** | [Docente / Manuel Alejandro Dominguez Guerrero] |
| **Versión** | 1.0 |
| **Relacionado con** | ADR-001, ADR-002 |

## 1. Partes interesadas y preocupaciones (ISO/IEC/IEEE 42010:2022)

| Parte interesada | Preocupaciones |
|---|---|
| Equipo de desarrollo | Clases pequeñas, comprensibles y fáciles de probar |
| Administración de la clínica | Que consultar o reportar no dependa de la lógica de agendamiento |
| Mantenimiento futuro | Bajo riesgo de regresión al evolucionar el sistema |

**Vistas afectadas:** vista lógica (estructura de clases y contratos).

## 2. Contexto y fuerzas

`GestorCitasOdontologicas` (L5-98) concentra 7 responsabilidades: disponibilidad, copago,
creación de la cita, multa, persistencia, notificación y estadísticas. Mide LCOM96b = 0.50,
complejidad ciclomática 10 en `AgendarCita` y 51 líneas lógicas. Su superficie pública mezcla
comandos y reportes (ISP). Además muta el estado de la entidad `Cita` desde fuera (L85-86),
lanza `Exception` genérica (L16) y no ofrece la función "consultar citas" que pide el enunciado.

## 3. Decisión

1. Un **servicio por caso de uso**: `AgendarCitaServicio`, `CancelarCitaServicio`, `ConsultarCitasServicio`, cada uno detrás de una interfaz (`IAgendarCita`, `ICancelarCita`, `IConsultarCitas`).
2. **Segregar interfaces por rol:**
   - Persistencia: `IRepositorioCitasEscritura` e `IRepositorioCitasLectura`.
   - Estadísticas: `IRegistradorEstadisticas` e `IConsultorEstadisticas`.
   - Notificación: `ICanalNotificacion` por tecnología.
3. Un **modelo de lectura** (`CitaResumen`) para consultas, sin reconstruir la entidad completa.
4. **Proteger la entidad:** `Cita.Programar(...)` y `Cita.Cancelar(...)` encapsulan el estado y la invariante "no se cancela dos veces". `Cita` ya no redacta notificaciones.
5. **Excepción tipada** `OdontologoNoDisponibleException` en lugar de `Exception`.
6. El notificador **usa el mensaje que recibe**, corrigiendo el contrato roto del legado.

## 4. Alternativas consideradas

| Alternativa | Ventajas | Desventajas | Decisión |
|---|---|---|---|
| A. Mantener `GestorCitas` como fachada que delega | Cambia poco para los clientes | Conserva la "interfaz gorda" (ISP) y el punto único de cambio | Descartada |
| B. Servicios por caso de uso + interfaces por rol | SRP e ISP claros; cada cliente depende de lo que usa | Más interfaces y servicios con varios colaboradores | **Elegida** |
| C. Un único `IRepositorio` con todas las operaciones | Menos interfaces | Obliga a quien solo lee a depender de escribir | Descartada |
| D. Mediador (p. ej. MediatR) | Desacopla aún más | Dependencia externa; excesivo para el alcance | Aplazada |

## 5. Fundamento

SRP: cada clase debe tener una sola razón de cambio. ISP: ningún cliente debe depender de métodos
que no usa; por eso `ConsultarCitasServicio` solo recibe la interfaz de lectura y no puede
modificar citas. Respecto a LSP, el legado no tenía herencia, así que no había una violación
formal por subtipado; la decisión corrige violaciones de **contrato** (mensaje ignorado,
dos fuentes de verdad, excepción genérica), que son las que impedirían sustituir componentes.

## 6. Consecuencias

**Positivas**
- LCOM96b de la clase principal: 0.50 → 0.00. Promedio de las clases con estado: 0.167 → 0.028.
- Clase de negocio más grande: 51 → 26 líneas.
- Se cubre el requisito "consultar citas".
- Las implementaciones (SQL/memoria, SMTP/Twilio/consola) son intercambiables sin tocar los servicios.

**Negativas / trade-offs**
- **El Ce sube** (5 → 11 en `AgendarCitaServicio`), pero todas las dependencias son interfaces o tipos del modelo (0 clases concretas).
- Más interfaces y más cableado en `ComposicionRaiz`.
- Hay que mantener dos representaciones: `Cita` (escritura) y `CitaResumen` (lectura).
- `EstadisticasEnMemoria` tiene LCOM96b = 0.33 porque dos contadores independientes comparten un candado. Podría dividirse en dos clases.
- Los servicios siguen coordinando 4-5 colaboradores: si crece más, es señal de revisar SRP.

**Riesgos y mitigaciones**
- *Servicios que acumulan dependencias:* se fija un umbral de revisión (ver Gobernanza).
- *Confusión entre `Cita` y `CitaResumen`:* se documenta el propósito de cada uno.

## 7. Verificación

- LCOM96b, Ca, Ce, I, A, D y complejidad ciclomática antes y después (informe de métricas, tabla comparativa).
- `ConsultarCitasServicio` solo recibe `IRepositorioCitasLectura`, que no tiene operaciones de escritura.
- El sistema ejecuta los flujos de agendar, cancelar, consultar y reportar con los resultados esperados.

## 8. Gobernanza

- **Responsable:** líder técnico del equipo.
- **Cambios:** una nueva función entra como un nuevo caso de uso con su interfaz, nunca como un método más en un servicio existente.
- **Umbral de revisión:** si un servicio supera 7 dependencias a interfaces, o su complejidad ciclomática pasa de 8, se revisa si debe dividirse.
- **Revisión periódica:** semestral o al agregar un caso de uso.

## 9. Trazabilidad

| Legado | Refactorizado |
|---|---|
| `AgendarCita` (L12-73) | `AgendarCitaServicio` (`IAgendarCita`) |
| `CancelarCita` (L75-93) | `CancelarCitaServicio` (`ICancelarCita`) |
| (no existía) | `ConsultarCitasServicio` (`IConsultarCitas`), `CitaResumen` |
| L9-10, L71, L91, L95-97 | `IRegistradorEstadisticas`, `IConsultorEstadisticas`, `EstadisticasEnMemoria` |
| L85-86 (cambio de estado) | `Cita.Cancelar(...)` |
| L16 (`Exception`) | `OdontologoNoDisponibleException` |