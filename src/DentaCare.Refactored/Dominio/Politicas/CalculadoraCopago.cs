using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Dominio.Politicas;

public sealed record SolicitudCopago(Paciente Paciente, Odontologo Odontologo, bool RequiereRadiografia);

public interface ICalculadoraCopago
{
    decimal Calcular(SolicitudCopago solicitud);
}

public sealed class CalculadoraCopago : ICalculadoraCopago
{
    private const decimal CostoConsultaBase = 100.0m;

    private readonly IReadOnlyDictionary<EspecialidadOdontologica, IReglaTarifaEspecialidad> _tarifas;
    private readonly IReadOnlyDictionary<TipoConvenio, IDescuentoConvenio> _descuentos;
    private readonly IReadOnlyList<IRecargoCopago> _recargos;

    public CalculadoraCopago(
        IEnumerable<IReglaTarifaEspecialidad> tarifas,
        IEnumerable<IDescuentoConvenio> descuentos,
        IEnumerable<IRecargoCopago> recargos)
    {
        _tarifas = tarifas.ToDictionary(t => t.Especialidad);
        _descuentos = descuentos.ToDictionary(d => d.Convenio);
        _recargos = recargos.ToList();
    }

    public decimal Calcular(SolicitudCopago solicitud)
    {
        if (!_tarifas.TryGetValue(solicitud.Odontologo.Especialidad, out IReglaTarifaEspecialidad? tarifa))
        {
            throw new InvalidOperationException($"No hay una tarifa registrada para la especialidad {solicitud.Odontologo.Especialidad}.");
        }

        if (!_descuentos.TryGetValue(solicitud.Paciente.TipoConvenio, out IDescuentoConvenio? descuento))
        {
            throw new InvalidOperationException($"No hay un descuento registrado para el convenio {solicitud.Paciente.TipoConvenio}.");
        }

        // Mismo orden que el legado: tarifa -> convenio -> recargos
        decimal copago = tarifa.CalcularTarifaBase(CostoConsultaBase);
        copago = descuento.Aplicar(copago);

        foreach (IRecargoCopago recargo in _recargos)
        {
            copago += recargo.Calcular(solicitud);
        }

        return copago;
    }
}