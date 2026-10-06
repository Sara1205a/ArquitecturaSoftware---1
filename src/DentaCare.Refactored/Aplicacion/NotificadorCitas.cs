using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Aplicacion;

public sealed class NotificadorCitas : INotificadorCitas
{
    private readonly IReadOnlyList<ICanalNotificacion> _canales;

    public NotificadorCitas(IEnumerable<ICanalNotificacion> canales)
    {
        _canales = canales.ToList();
    }

    public void NotificarCitaProgramada(Cita cita)
    {
        string asunto = "Confirmación de Cita Odontológica";
        string mensaje = $"Su cita #{cita.Id} quedó programada para el {cita.FechaHora:yyyy-MM-dd HH:mm}. Copago estimado: ${cita.CopagoCalculado:N2}.";

        foreach (ICanalNotificacion canal in _canales)
        {
            canal.Enviar(cita.Paciente, asunto, mensaje);
        }
    }

    public void NotificarCitaCancelada(Cita cita)
    {
        string asunto = "Cancelación de Cita Odontológica";
        string mensaje = $"Su cita #{cita.Id} ha sido CANCELADA. Penalización aplicada: ${cita.PenalizacionCancelacion:N2}.";

        foreach (ICanalNotificacion canal in _canales)
        {
            canal.Enviar(cita.Paciente, asunto, mensaje);
        }
    }
}