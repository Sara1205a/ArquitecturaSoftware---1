namespace DentaCare.Refactored.Dominio.Modelo;

public class OdontologoNoDisponibleException : Exception
{
    public OdontologoNoDisponibleException(string nombreOdontologo, DateTime fechaHora)
        : base($"El odontólogo {nombreOdontologo} no tiene disponibilidad en el horario seleccionado ({fechaHora:yyyy-MM-dd HH:mm}).")
    {
    }
}