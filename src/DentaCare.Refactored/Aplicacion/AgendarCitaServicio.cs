using DentaCare.Refactored.Dominio.Modelo;
using DentaCare.Refactored.Dominio.Politicas;

namespace DentaCare.Refactored.Aplicacion;

public sealed class AgendarCitaServicio : IAgendarCita
{
    private readonly IValidadorDisponibilidad _disponibilidad;
    private readonly ICalculadoraCopago _calculadoraCopago;
    private readonly IRepositorioCitasEscritura _repositorio;
    private readonly INotificadorCitas _notificador;
    private readonly IRegistradorEstadisticas _estadisticas;

    public AgendarCitaServicio(
        IValidadorDisponibilidad disponibilidad,
        ICalculadoraCopago calculadoraCopago,
        IRepositorioCitasEscritura repositorio,
        INotificadorCitas notificador,
        IRegistradorEstadisticas estadisticas)
    {
        _disponibilidad = disponibilidad;
        _calculadoraCopago = calculadoraCopago;
        _repositorio = repositorio;
        _notificador = notificador;
        _estadisticas = estadisticas;
    }

    public Cita Ejecutar(Paciente paciente, Odontologo odontologo, DateTime fechaHora, bool requiereRadiografia)
    {
        if (!_disponibilidad.EstaDisponible(odontologo, fechaHora))
        {
            throw new OdontologoNoDisponibleException(odontologo.Nombre, fechaHora);
        }

        decimal copago = _calculadoraCopago.Calcular(new SolicitudCopago(paciente, odontologo, requiereRadiografia));
        Cita cita = Cita.Programar(paciente, odontologo, fechaHora, copago);

        _repositorio.Guardar(cita);
        _notificador.NotificarCitaProgramada(cita);
        _estadisticas.RegistrarCitaAgendada(copago);

        return cita;
    }
}