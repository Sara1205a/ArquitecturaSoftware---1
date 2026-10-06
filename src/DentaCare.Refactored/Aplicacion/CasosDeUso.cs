using DentaCare.Refactored.Dominio.Modelo;

namespace DentaCare.Refactored.Aplicacion;

public interface IAgendarCita
{
	Cita Ejecutar(Paciente paciente, Odontologo odontologo, DateTime fechaHora, bool requiereRadiografia);
}

public interface ICancelarCita
{
	decimal Ejecutar(Cita cita, DateTime fechaHoraCancelacion);
}

public interface IConsultarCitas
{
	CitaResumen? PorId(string citaId);
	IReadOnlyList<CitaResumen> PorPaciente(string pacienteId);
}