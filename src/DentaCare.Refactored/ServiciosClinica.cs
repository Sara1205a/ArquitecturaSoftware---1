using DentaCare.Refactored.Aplicacion;

namespace DentaCare.Refactored;

public sealed record ServiciosClinica(
    IAgendarCita Agendar,
    ICancelarCita Cancelar,
    IConsultarCitas Consultar,
    IConsultorEstadisticas Estadisticas);