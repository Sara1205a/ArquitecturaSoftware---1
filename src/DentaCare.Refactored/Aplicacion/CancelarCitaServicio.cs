using DentaCare.Refactored.Dominio.Modelo;
using DentaCare.Refactored.Dominio.Politicas;

namespace DentaCare.Refactored.Aplicacion;

public sealed class CancelarCitaServicio : ICancelarCita
{
    private readonly ICalculadoraPenalizacion _calculadoraPenalizacion;
    private readonly IRepositorioCitasEscritura _repositorio;
    private readonly INotificadorCitas _notificador;
    private readonly IRegistradorEstadisticas _estadisticas;

    public CancelarCitaServicio(
        ICalculadoraPenalizacion calculadoraPenalizacion,
        IRepositorioCitasEscritura repositorio,
        INotificadorCitas notificador,
        IRegistradorEstadisticas estadisticas)
    {
        _calculadoraPenalizacion = calculadoraPenalizacion;
        _repositorio = repositorio;
        _notificador = notificador;
        _estadisticas = estadisticas;
    }

    public decimal Ejecutar(Cita cita, DateTime fechaHoraCancelacion)
    {
        decimal penalizacion = _calculadoraPenalizacion.Calcular(cita, fechaHoraCancelacion);

        cita.Cancelar(penalizacion);

        _repositorio.ActualizarCancelacion(cita);
        _notificador.NotificarCitaCancelada(cita);
        _estadisticas.RegistrarCancelacion();

        return penalizacion;
    }
}