namespace DentaCare.Refactored.Dominio.Modelo;

public class Odontologo
{
    public Odontologo(string id, string nombre, EspecialidadOdontologica especialidad, bool estaDisponible)
    {
        Id = id;
        Nombre = nombre;
        Especialidad = especialidad;
        EstaDisponible = estaDisponible;
    }

    public string Id { get; }
    public string Nombre { get; }
    public EspecialidadOdontologica Especialidad { get; }
    public bool EstaDisponible { get; set; }
}