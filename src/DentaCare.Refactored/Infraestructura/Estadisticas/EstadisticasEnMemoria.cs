using DentaCare.Refactored.Aplicacion;

namespace DentaCare.Refactored.Infraestructura.Estadisticas;

public sealed class EstadisticasEnMemoria : IRegistradorEstadisticas, IConsultorEstadisticas
{
    private readonly object _candado = new();
    private decimal _totalRecaudado;
    private int _totalCanceladas;

    public void RegistrarCitaAgendada(decimal copago)
    {
        lock (_candado) { _totalRecaudado += copago; }
    }

    public void RegistrarCancelacion()
    {
        lock (_candado) { _totalCanceladas++; }
    }

    public decimal ObtenerTotalRecaudado()
    {
        lock (_candado) { return _totalRecaudado; }
    }

    public int ObtenerTotalCanceladas()
    {
        lock (_candado) { return _totalCanceladas; }
    }
}