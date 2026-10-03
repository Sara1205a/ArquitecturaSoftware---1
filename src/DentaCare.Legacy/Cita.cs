using System;
using System.Collections.Generic;
using System.Text;

namespace DentaCare.Legacy
{
    public class Cita
    {
        public string Id { get; set; }
        public Paciente Paciente { get; set; }
        public Odontologo Odontologo { get; set; }
        public DateTime FechaHora { get; set; }
        public decimal CopagoCalculado { get; set; }
        public string Estado { get; set; } // 'PROGRAMADA', 'CANCELADA'
        public decimal PenalizacionCancelacion { get; set; }
        public string GenerarTextoConfirmacion() {
            return $"CITA #{Id} - Paciente: {Paciente?.NombreCompleto}, Fecha: {FechaHora:yyyy-MM-dd HH:mm}, Copago: ${CopagoCalculado}"; 
        }
    }
}
