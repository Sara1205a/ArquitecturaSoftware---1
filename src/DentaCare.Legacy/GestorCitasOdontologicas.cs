using System.Data.SqlClient;

namespace DentaCare.Legacy
{
    public class GestorCitasOdontologicas
    {
        private SqlServerEjecutor _db = new SqlServerEjecutor();
        private NotificacionServicio _notificador = new NotificacionServicio();
        private decimal _totalRecaudadoMes = 0;
        private int _totalCitasCanceladas = 0;

        public Cita AgendarCita(Paciente paciente, Odontologo odontologo, DateTime fechaHora, int tipoEspecialidad, bool requiereRadiografia)
        {
            if (!odontologo.EstaDisponible)
            {
                throw new Exception("El odontólogo no tiene disponibilidad en el horario seleccionado.");
            }

            decimal costoConsulta = 100.0m;
            decimal copagoFinal = 0;

            if (tipoEspecialidad == 1) // Ortodoncia
            {
                copagoFinal = costoConsulta * 1.2m;
            }
            else if (tipoEspecialidad == 2) // Endodoncia
            {
                copagoFinal = costoConsulta * 1.8m;
            }
            else if (tipoEspecialidad == 3) // Cirugía
            {
                copagoFinal = costoConsulta * 2.5m;
            }
            else if (tipoEspecialidad == 4) // Odontopediatría
            {
                copagoFinal = costoConsulta * 1.1m;
            }

            // Descuentos por convenio
            if (paciente.TipoConvenio == 2) // EPS
            {
                copagoFinal *= 0.30m;
            }
            else if (paciente.TipoConvenio == 3) // Prepagada
            {
                copagoFinal *= 0.10m;
            }

            if (paciente.EsPrimeraVez)
            {
                copagoFinal += 20.0m;
            }

            if (requiereRadiografia)
            {
                copagoFinal += 35.0m;
            }
            Cita nuevaCita = new Cita
            {
                Id = Guid.NewGuid().ToString().Substring(0, 8),
                Paciente = paciente,
                Odontologo = odontologo,
                FechaHora = fechaHora,
                CopagoCalculado = copagoFinal,
                Estado = "PROGRAMADA"
            };
            
            _db.GuardarCita(nuevaCita); 
            _notificador.EnviarEmailYSms(nuevaCita, nuevaCita.GenerarTextoConfirmacion()); 

            _totalRecaudadoMes += copagoFinal; 
            return nuevaCita;
        }

        public decimal CancelarCita(Cita cita, DateTime fechaHoraCancelacion)
        {
            TimeSpan diferenciaTiempo = cita.FechaHora - fechaHoraCancelacion; decimal penalizacion = 0; 
            
            if (diferenciaTiempo.TotalHours < 24)
            {
                penalizacion = 50.0m; if (cita.Odontologo.EspecialidadId == 3) // Cirugía
                penalizacion += 40.0m;
            }

            cita.Estado = "CANCELADA"; 
            cita.PenalizacionCancelacion = penalizacion; 

            _db.ActualizarCancelacion(cita.Id, penalizacion); 
            _notificador.EnviarEmailYSms(cita, $"Su cita #{cita.Id} ha sido CANCELADA."); 

            _totalCitasCanceladas++;             
            return penalizacion;
        }

        public decimal ObtenerTotalRecaudado() => _totalRecaudadoMes;

        public int ObtenerTotalCanceladas() => _totalCitasCanceladas;
    }
}