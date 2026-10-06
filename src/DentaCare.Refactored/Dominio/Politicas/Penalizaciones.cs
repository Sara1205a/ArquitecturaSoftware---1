using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public interface IReglaPenalizacion
{
    decimal Calcular(Cita cita, DateTime fechaHoraCancelacion);
}

public sealed class PenalizacionCancelacionTardia : IReglaPenalizacion
{
    public const double HorasLimite = 24;
    public const decimal Monto = 50.0m;

    public decimal Calcular(Cita cita, DateTime fechaHoraCancelacion)
    {
        double horasAntelacion = (cita.FechaHora - fechaHoraCancelacion).TotalHours;
        return horasAntelacion < HorasLimite ? Monto : 0m;
    }
}

public sealed class PenalizacionAdicionalCirugia : IReglaPenalizacion
{
    public const decimal Monto = 40.0m;

    public decimal Calcular(Cita cita, DateTime fechaHoraCancelacion)
    {
        double horasAntelacion = (cita.FechaHora - fechaHoraCancelacion).TotalHours;
        bool esTardia = horasAntelacion < PenalizacionCancelacionTardia.HorasLimite;
        bool esCirugia = cita.Odontologo.Especialidad == EspecialidadOdontologica.Cirugia;
        return esTardia && esCirugia ? Monto : 0m;
    }
}