using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DentaCare.Legacy
{
    public class SqlServerEjecutor
    {
        private string _cadenaConexionSql = "Server=sql.dentacare.com;Database=DentalDb;User Id=sa;Password=DentalPass123!;"; 
        public void GuardarCita(Cita cita) 
        {
            using (SqlConnection conn = new SqlConnection(_cadenaConexionSql))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Citas (PacienteId, OdontologoId, Fecha, Copago) VALUES (@p, @o, @f, @c)", conn);
                cmd.Parameters.AddWithValue("@p", cita.Paciente.Id);
                cmd.Parameters.AddWithValue("@o", cita.Odontologo.Id);
                cmd.Parameters.AddWithValue("@f", cita.FechaHora);
                cmd.Parameters.AddWithValue("@c", cita.CopagoCalculado);
                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarCancelacion(string citaId, decimal penalizacion) 
        {
            using (SqlConnection conn = new SqlConnection(_cadenaConexionSql))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Citas SET Estado = 'CANCELADA', Penalizacion = @pen WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@pen", penalizacion);
                cmd.Parameters.AddWithValue("@id", citaId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
