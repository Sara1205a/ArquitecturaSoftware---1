# ADR-002: Ajuste de políticas de cancelación y tarifas mediante estrategias

| Campo | Valor |
|---|---|
| **ID** | ADR-002 |
| **Estado** | Aceptado |
| **Fecha** | 2026-10-06 |
| **Autores** | [Sara Arboleda Uribe] |
| **Revisores** | [Docente / Manuel Alejandro Dominguez Guerrero] |
| **Versión** | 1.0 |
| **Relacionado con** | ADR-001, ADR-003 |

## 1. Partes interesadas y preocupaciones (ISO/IEC/IEEE 42010:2022)

| Parte interesada | Preocupaciones |
|---|---|
| Gerencia / finanzas de la clínica | Poder cambiar tarifas y multas sin riesgo de romper otras reglas |
| Equipo de desarrollo | Agregar reglas sin modificar código ya probado |
| Pacientes | Cobros correctos y predecibles |
| Auditoría | Trazabilidad de cada fórmula de cobro |

**Vistas afectadas:** vista lógica (módulo de políticas de negocio).

## 2. Contexto y fuerzas

El cálculo del copago y de la multa vive en cadenas `if / else if` dentro del Gestor:
especialidad (L22-37), convenio (L40-47), recargos (L49-57) y multa (L79-83). Los códigos
numéricos 1, 2, 3, 4 solo están explicados con comentarios y los factores están incrustados.
Cada especialidad, convenio o regla nueva obliga a editar la misma clase. Una especialidad
desconocida produce copago 0 sin avisar. Además, la especialidad llega por dos vías que pueden
contradecirse: el parámetro `tipoEspecialidad` (L12) y `odontologo.EspecialidadId` (L81).

## 3. Decisión

1. Aplicar el patrón **Strategy**: una interfaz por tipo de regla y una clase por regla.
   - `IReglaTarifaEspecialidad`: `TarifaOrtodoncia`, `TarifaEndodoncia`, `TarifaCirugia`, `TarifaOdontopediatria`.
   - `IDescuentoConvenio`: `DescuentoParticular`, `DescuentoEps`, `DescuentoPrepagada`.
   - `IRecargoCopago`: `RecargoPrimeraVez`, `RecargoRadiografia`.
   - `IReglaPenalizacion`: `PenalizacionCancelacionTardia`, `PenalizacionAdicionalCirugia`.
2. `CalculadoraCopago` y `CalculadoraPenalizacion` orquestan las reglas sin conocerlas. Las reglas se registran en `ComposicionRaiz`.
3. **Se conserva el orden y las fórmulas del legado:** tarifa, luego convenio, luego recargos.
4. Reemplazar los números mágicos por enumeraciones (`EspecialidadOdontologica`, `TipoConvenio`, `EstadoCita`).
5. **Fallar rápido:** si no hay regla para una especialidad o convenio, se lanza `InvalidOperationException`.
6. **Una sola fuente de verdad:** la especialidad se toma de `Odontologo.Especialidad`. Se elimina el parámetro `tipoEspecialidad`.

## 4. Alternativas consideradas

| Alternativa | Ventajas | Desventajas | Decisión |
|---|---|---|---|
| A. `switch` expression o diccionario en una clase | Compacto | Sigue siendo modificar la clase al agregar reglas | Descartada |
| B. Strategy: una clase por regla | Extensible sin modificar; cada regla se prueba sola | Muchas clases pequeñas | **Elegida** |
| C. Tarifas en base de datos (data-driven) | Cambio sin recompilar | Requiere esquema, gestión de datos y validaciones | Aplazada |
| D. Motor de reglas externo | Máxima flexibilidad | Complejidad desproporcionada | Descartada |

## 5. Fundamento

OCP: el sistema debe estar abierto a extensión y cerrado a modificación. Con Strategy, agregar una
especialidad no toca `CalculadoraCopago`. Los números mágicos se vuelven tipos que el compilador
verifica. Fallar rápido evita cobrar 0 en silencio.

## 6. Consecuencias

**Positivas**
- Agregar una especialidad: 1 clase de ~3 líneas + 1 valor del enum + 1 línea de registro. Ninguna clase de negocio existente se modifica.
- Complejidad ciclomática máxima por método: 10 → 4.
- Cada regla se puede probar por separado.

**Negativas / trade-offs**
- Más clases (un aumento notable de archivos).
- **Cambios de comportamiento deliberados:** (a) especialidad o convenio sin regla ahora lanza error (antes devolvía 0); (b) `AgendarCita` ya no recibe `tipoEspecialidad`, lo que **cambia la firma pública**; (c) cancelar una cita ya cancelada lanza excepción (antes duplicaba multa y contador).
- Olvidar registrar una regla no da error de compilación, sino de ejecución (se mitiga con el fallo inmediato).
- El orden importa: el descuento se aplica antes que los recargos. Cambiarlo altera los importes.
- Acoplamiento menor: `PenalizacionAdicionalCirugia` usa la constante `HorasLimite` de `PenalizacionCancelacionTardia`.

**Riesgos y mitigaciones**
- *Divergencia con el legado:* se verifica con casos esperados (por ejemplo Cirugía + EPS + primera vez + radiografía = 130.00; cancelar a 12 h una cirugía = 90.00).
- *Valores de negocio en el código:* se aplaza a la alternativa C si cambian con frecuencia.

## 7. Verificación

- Los 7 casos de copago y los 5 de penalización del informe producen los valores esperados.
- La demostración da copago 130.00 y penalización 90.00.
- No existe `if` o `switch` sobre especialidad o convenio fuera de las propias reglas.

## 8. Gobernanza

- **Dueño de las cifras:** gerencia/finanzas aprueba cualquier cambio de tarifa, convenio o multa.
- **Dueño técnico:** equipo de desarrollo implementa el cambio y actualiza los casos de prueba.
- **Proceso:** pull request con revisión; los importes nuevos requieren aprobación escrita del dueño de negocio.
- **Revisión periódica:** trimestral o con cada cambio de convenio.
- **Criterio para reabrir:** si las tarifas cambian más de una vez por trimestre, evaluar la alternativa C.

## 9. Trazabilidad

| Legado | Refactorizado |
|---|---|
| L22-37 (especialidad) | `IReglaTarifaEspecialidad` + 4 clases `Tarifa*` |
| L40-47 (convenio) | `IDescuentoConvenio` + 3 clases `Descuento*` |
| L49-57 (recargos) | `IRecargoCopago` + `RecargoPrimeraVez`, `RecargoRadiografia` |
| L79-83 (multa) | `IReglaPenalizacion` + 2 clases `Penalizacion*` |