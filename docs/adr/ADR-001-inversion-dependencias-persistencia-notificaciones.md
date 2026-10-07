# ADR-001: Inversión de dependencias para persistencia y notificaciones

| Campo | Valor |
|---|---|
| **ID** | ADR-001 |
| **Estado** | Aceptado |
| **Fecha** | 2026-10-06 |
| **Autores** | [Sara Arboleda Uribe] |
| **Revisores** | [Docente / Manuel Alejandro Dominguez Guerrero] |
| **Versión** | 1.0 |
| **Relacionado con** | ADR-003 |

## 1. Partes interesadas y preocupaciones (ISO/IEC/IEEE 42010:2022)

| Parte interesada | Preocupaciones |
|---|---|
| Equipo de desarrollo | Mantenibilidad, testabilidad, costo de cambiar de proveedor |
| Administración de la clínica | Continuidad del servicio, no depender de un único proveedor |
| Pacientes | Recibir notificaciones correctas; privacidad de sus datos |
| Seguridad / TI | Credenciales fuera del código fuente y de los logs |

**Vistas afectadas:** vista lógica (módulos y dependencias), vista de despliegue/configuración (secretos) y vista de seguridad.

## 2. Contexto y fuerzas

`GestorCitasOdontologicas` crea con `new` sus propias dependencias concretas
(`SqlServerEjecutor` en L7 y `NotificacionServicio` en L8) y las usa en L68-69 y L88-89.
La lógica de negocio no puede probarse sin SQL Server y SMTP reales. Además:
- La contraseña de `sa` está en `SqlServerEjecutor.cs` L10.
- El servidor SMTP y la API key de Twilio están en `NotificacionServicio.cs` L10-11, y la API key se imprime en consola (L21).
- El INSERT no guarda `Id` ni `Estado`, pero el UPDATE filtra por `Id`.
- La cancelación notifica "quedó programada" porque el método ignora el mensaje recibido.

**Medición base:** el Gestor tiene Ce = 5, con 2 dependencias concretas de infraestructura.

## 3. Decisión

1. Definir **puertos** (interfaces) en la capa Aplicación: `IRepositorioCitasEscritura`, `IRepositorioCitasLectura`, `ICanalNotificacion`, `INotificadorCitas`.
2. Implementarlos como **adaptadores** en la capa Infraestructura: `SqlServerRepositorioCitas`, `RepositorioCitasEnMemoria`, `CanalCorreoSmtp`, `CanalSmsTwilio`, `CanalConsola`.
3. Inyectar las dependencias **por constructor**. `ComposicionRaiz` es el único lugar con `new` de clases concretas.
4. Sacar los secretos del código: se leen de variables de entorno (`DENTACARE_SQL`, `TWILIO_API_KEY`, `DENTACARE_SMTP`).
5. Un canal por tecnología de envío; `NotificadorCitas` reparte el mensaje por todos los canales registrados.

## 4. Alternativas consideradas

| Alternativa | Ventajas | Desventajas | Decisión |
|---|---|---|---|
| A. Extraer clases sin interfaces | Menos código | Sigue acoplado a lo concreto; no se puede sustituir ni probar | Descartada |
| B. Puertos y adaptadores con DI manual | Cumple DIP; sin dependencias externas | Más archivos; el cableado es manual | **Elegida** |
| C. Contenedor DI (Microsoft.Extensions) + EF Core | Menos cableado manual | Dependencias nuevas; excesivo para el alcance | Aplazada |
| D. Fachada o Singleton global | Sencillo | Estado global oculto, difícil de probar | Descartada |

## 5. Fundamento

El módulo de alto nivel (casos de uso) no debe depender de detalles de bajo nivel (SQL, SMTP). Las
interfaces pertenecen a la capa interna, de modo que las dependencias apuntan hacia adentro.
La opción B logra el desacoplo sin añadir librerías, y permite ejecutar el sistema sin infraestructura real.

## 6. Consecuencias

**Positivas**
- Ce hacia infraestructura concreta: 2 → 0. Dependencias de Dominio/Aplicación hacia Infraestructura: 0.
- Cambiar de proveedor SMS o de base de datos no toca la lógica de negocio.
- Se corrigen el defecto del INSERT, el mensaje de cancelación y la fuga de la API key.

**Negativas / trade-offs**
- Más archivos e indirección; hay que entender el flujo para seguirlo.
- El cableado manual en `ComposicionRaiz` crece con cada componente nuevo.
- Variables de entorno no es un gestor de secretos: es una mejora, no una solución definitiva.
- Si un canal falla, el error se propaga y los canales siguientes no se envían (igual que el legado).

**Riesgos y mitigaciones**
- *Se asumió que `Id` y `Estado` existen en la tabla `Citas` y que `Id` es texto.* Se mitiga validando contra el DDL real.
- *Se usa `System.Data.SqlClient`, que está en desuso.* Se mitiga migrando a `Microsoft.Data.SqlClient` (solo cambia la capa de Infraestructura).

## 7. Verificación

- Dominio y Aplicación no referencian el namespace `Infraestructura` (revisión por búsqueda de texto o test de arquitectura).
- Ce hacia infraestructura concreta de cada servicio = 0 (informe de métricas).
- El sistema compila y corre en modo demostración sin SQL Server ni SMTP.

## 8. Gobernanza

- **Responsable:** líder técnico del equipo.
- **Cambios:** toda modificación a un puerto o a la raíz de composición va por pull request con revisión de un segundo integrante.
- **Revisión periódica:** semestral, o cuando se agregue un canal o cambie el proveedor de datos.
- **Criterio para reabrir esta decisión:** si el cableado manual supera unas 15 dependencias, evaluar un contenedor DI (alternativa C).

## 9. Trazabilidad

| Legado | Refactorizado |
|---|---|
| `GestorCitasOdontologicas` L7-8, L68-69, L88-89 | `IRepositorio*`, `INotificadorCitas`, inyección por constructor |
| `SqlServerEjecutor` | `SqlServerRepositorioCitas`, `RepositorioCitasEnMemoria` |
| `NotificacionServicio` | `NotificadorCitas` + `CanalCorreoSmtp`, `CanalSmsTwilio`, `CanalConsola` |