namespace DentaCare.Refactored.Dominio.Modelo;

public class Paciente
{
    public Paciente(string id, string nombreCompleto, string correo, string celular,
                    TipoConvenio tipoConvenio, bool esPrimeraVez)
    {
        Id = id;
        NombreCompleto = nombreCompleto;
        Correo = correo;
        Celular = celular;
        TipoConvenio = tipoConvenio;
        EsPrimeraVez = esPrimeraVez;
    }

    public string Id { get; }
    public string NombreCompleto { get; }
    public string Correo { get; }
    public string Celular { get; }
    public TipoConvenio TipoConvenio { get; }
    public bool EsPrimeraVez { get; }
}