using System.Collections.Concurrent;
using DentaCare.Refactored.Aplicacion;
using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Infraestructura.Persistencia;

public sealed class RepositorioCitasEnMemoria : IRepositorioCitasEscritura, IRepositorioCitasLectura
{
    private readonly ConcurrentDictionary<string, Cita> _citas = new();

    public void Guardar(Cita cita)
    {
        _citas[cita.Id] = cita;
    }

    public void ActualizarCancelacion(Cita cita)
    {
        _citas[cita.Id] = cita;
    }

    public CitaResumen? ObtenerPorId(string citaId)
    {
        return _citas.TryGetValue(citaId, out Cita? cita) ? Resumir(cita) : null;
    }

    public IReadOnlyList<CitaResumen> ObtenerPorPaciente(string pacienteId)
    {
        return _citas.Values
            .Where(c => c.Paciente.Id == pacienteId)
            .OrderBy(c => c.FechaHora)
            .Select(Resumir)
            .ToList();
    }

    private static CitaResumen Resumir(Cita cita) => new CitaResumen(
        cita.Id, cita.Paciente.Id, cita.Odontologo.Id, cita.FechaHora,
        cita.CopagoCalculado, cita.Estado, cita.PenalizacionCancelacion);
}