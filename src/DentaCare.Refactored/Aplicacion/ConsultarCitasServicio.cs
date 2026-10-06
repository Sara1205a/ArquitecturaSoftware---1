namespace DentaCare.Refactored.Aplicacion;

public sealed class ConsultarCitasServicio : IConsultarCitas
{
    private readonly IRepositorioCitasLectura _repositorio;

    public ConsultarCitasServicio(IRepositorioCitasLectura repositorio)
    {
        _repositorio = repositorio;
    }

    public CitaResumen? PorId(string citaId) => _repositorio.ObtenerPorId(citaId);

    public IReadOnlyList<CitaResumen> PorPaciente(string pacienteId) => _repositorio.ObtenerPorPaciente(pacienteId);
}