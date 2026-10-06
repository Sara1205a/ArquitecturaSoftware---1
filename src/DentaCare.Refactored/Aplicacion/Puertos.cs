using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Aplicacion;

public sealed record CitaResumen(
    string Id,
    string PacienteId,
    string OdontologoId,
    DateTime FechaHora,
    decimal Copago,
    EstadoCita Estado,
    decimal Penalizacion);

// ---- Persistencia (ISP: escritura y lectura separadas) ----
public interface IRepositorioCitasEscritura
{
    void Guardar(Cita cita);
    void ActualizarCancelacion(Cita cita);
}

public interface IRepositorioCitasLectura
{
    CitaResumen? ObtenerPorId(string citaId);
    IReadOnlyList<CitaResumen> ObtenerPorPaciente(string pacienteId);
}

// ---- Notificaciones ----
public interface ICanalNotificacion
{
    string Nombre { get; }
    void Enviar(Paciente destinatario, string asunto, string mensaje);
}

public interface INotificadorCitas
{
    void NotificarCitaProgramada(Cita cita);
    void NotificarCitaCancelada(Cita cita);
}

// ---- Estadísticas (ISP: registrar y consultar separados) ----
public interface IRegistradorEstadisticas
{
    void RegistrarCitaAgendada(decimal copago);
    void RegistrarCancelacion();
}

public interface IConsultorEstadisticas
{
    decimal ObtenerTotalRecaudado();
    int ObtenerTotalCanceladas();
}