using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public interface ICalculadoraPenalizacion
{
    decimal Calcular(Cita cita, DateTime fechaHoraCancelacion);
}

public sealed class CalculadoraPenalizacion : ICalculadoraPenalizacion
{
    private readonly IReadOnlyList<IReglaPenalizacion> _reglas;

    public CalculadoraPenalizacion(IEnumerable<IReglaPenalizacion> reglas)
    {
        _reglas = reglas.ToList();
    }

    public decimal Calcular(Cita cita, DateTime fechaHoraCancelacion)
    {
        return _reglas.Sum(regla => regla.Calcular(cita, fechaHoraCancelacion));
    }
}