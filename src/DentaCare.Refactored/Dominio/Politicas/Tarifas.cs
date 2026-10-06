using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public interface IReglaTarifaEspecialidad
{
    EspecialidadOdontologica Especialidad { get; }
    decimal CalcularTarifaBase(decimal costoConsulta);
}

public sealed class TarifaOrtodoncia : IReglaTarifaEspecialidad
{
    public EspecialidadOdontologica Especialidad => EspecialidadOdontologica.Ortodoncia;
    public decimal CalcularTarifaBase(decimal costoConsulta) => costoConsulta * 1.2m;
}

public sealed class TarifaEndodoncia : IReglaTarifaEspecialidad
{
    public EspecialidadOdontologica Especialidad => EspecialidadOdontologica.Endodoncia;
    public decimal CalcularTarifaBase(decimal costoConsulta) => costoConsulta * 1.8m;
}

public sealed class TarifaCirugia : IReglaTarifaEspecialidad
{
    public EspecialidadOdontologica Especialidad => EspecialidadOdontologica.Cirugia;
    public decimal CalcularTarifaBase(decimal costoConsulta) => costoConsulta * 2.5m;
}

public sealed class TarifaOdontopediatria : IReglaTarifaEspecialidad
{
    public EspecialidadOdontologica Especialidad => EspecialidadOdontologica.Odontopediatria;
    public decimal CalcularTarifaBase(decimal costoConsulta) => costoConsulta * 1.1m;
}