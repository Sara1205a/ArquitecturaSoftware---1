using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public interface IValidadorDisponibilidad
{
    bool EstaDisponible(Odontologo odontologo, DateTime fechaHora);
}

public sealed class ValidadorDisponibilidadOdontologo : IValidadorDisponibilidad
{
    public bool EstaDisponible(Odontologo odontologo, DateTime fechaHora) => odontologo.EstaDisponible;
}