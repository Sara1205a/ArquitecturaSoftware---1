namespace DentaCare.Refactored.Dominio.Modelo;

public class Cita
{
    private Cita(string id, Paciente paciente, Odontologo odontologo, DateTime fechaHora, decimal copagoCalculado)
    {
        Id = id;
        Paciente = paciente;
        Odontologo = odontologo;
        FechaHora = fechaHora;
        CopagoCalculado = copagoCalculado;
        Estado = EstadoCita.Programada;
    }

    public string Id { get; }
    public Paciente Paciente { get; }
    public Odontologo Odontologo { get; }
    public DateTime FechaHora { get; }
    public decimal CopagoCalculado { get; }
    public EstadoCita Estado { get; private set; }
    public decimal PenalizacionCancelacion { get; private set; }

    public static Cita Programar(Paciente paciente, Odontologo odontologo, DateTime fechaHora, decimal copagoCalculado)
    {
        string id = Guid.NewGuid().ToString("N").Substring(0, 8);
        return new Cita(id, paciente, odontologo, fechaHora, copagoCalculado);
    }

    public void Cancelar(decimal penalizacion)
    {
        if (Estado == EstadoCita.Cancelada)
        {
            throw new InvalidOperationException($"La cita #{Id} ya se encuentra cancelada.");
        }

        Estado = EstadoCita.Cancelada;
        PenalizacionCancelacion = penalizacion;
    }
}