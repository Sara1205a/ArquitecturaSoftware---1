using System.Data.SqlClient;
using DentaCare.Refactored.Aplicacion;
using DentaCare.Refactored.Dominio.Modelo;
using DentaCare.Refactored.Infraestructura.Configuracion;

namespace DentaCare.Refactored.Infraestructura.Persistencia;

public sealed class SqlServerRepositorioCitas : IRepositorioCitasEscritura, IRepositorioCitasLectura
{
    private readonly string _cadenaConexion;

    public SqlServerRepositorioCitas(OpcionesSqlServer opciones)
    {
        _cadenaConexion = opciones.CadenaConexion;
    }

    public void Guardar(Cita cita)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqlCommand(
            "INSERT INTO Citas (Id, PacienteId, OdontologoId, Fecha, Copago, Estado) VALUES (@id, @p, @o, @f, @c, @e)",
            conexion);
        comando.Parameters.AddWithValue("@id", cita.Id);
        comando.Parameters.AddWithValue("@p", cita.Paciente.Id);
        comando.Parameters.AddWithValue("@o", cita.Odontologo.Id);
        comando.Parameters.AddWithValue("@f", cita.FechaHora);
        comando.Parameters.AddWithValue("@c", cita.CopagoCalculado);
        comando.Parameters.AddWithValue("@e", cita.Estado.ToString().ToUpperInvariant());
        comando.ExecuteNonQuery();
    }

    public void ActualizarCancelacion(Cita cita)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqlCommand(
            "UPDATE Citas SET Estado = @e, Penalizacion = @pen WHERE Id = @id",
            conexion);
        comando.Parameters.AddWithValue("@e", cita.Estado.ToString().ToUpperInvariant());
        comando.Parameters.AddWithValue("@pen", cita.PenalizacionCancelacion);
        comando.Parameters.AddWithValue("@id", cita.Id);
        comando.ExecuteNonQuery();
    }

    public CitaResumen? ObtenerPorId(string citaId)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqlCommand(
            "SELECT Id, PacienteId, OdontologoId, Fecha, Copago, Estado, Penalizacion FROM Citas WHERE Id = @id",
            conexion);
        comando.Parameters.AddWithValue("@id", citaId);
        using var lector = comando.ExecuteReader();
        return lector.Read() ? Mapear(lector) : null;
    }

    public IReadOnlyList<CitaResumen> ObtenerPorPaciente(string pacienteId)
    {
        var resultado = new List<CitaResumen>();
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var comando = new SqlCommand(
            "SELECT Id, PacienteId, OdontologoId, Fecha, Copago, Estado, Penalizacion FROM Citas WHERE PacienteId = @p ORDER BY Fecha",
            conexion);
        comando.Parameters.AddWithValue("@p", pacienteId);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            resultado.Add(Mapear(lector));
        }

        return resultado;
    }

    private static CitaResumen Mapear(SqlDataReader lector)
    {
        EstadoCita estado = lector.IsDBNull(5)
            ? EstadoCita.Programada
            : Enum.Parse<EstadoCita>(Convert.ToString(lector[5]) ?? string.Empty, ignoreCase: true);

        return new CitaResumen(
            Convert.ToString(lector[0]) ?? string.Empty,
            Convert.ToString(lector[1]) ?? string.Empty,
            Convert.ToString(lector[2]) ?? string.Empty,
            Convert.ToDateTime(lector[3]),
            Convert.ToDecimal(lector[4]),
            estado,
            lector.IsDBNull(6) ? 0m : Convert.ToDecimal(lector[6]));
    }
}