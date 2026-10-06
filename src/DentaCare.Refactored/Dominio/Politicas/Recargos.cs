namespace DentaCare.Refactored.Dominio.Politicas;

public interface IRecargoCopago
{
    decimal Calcular(SolicitudCopago solicitud);
}

public sealed class RecargoPrimeraVez : IRecargoCopago
{
    public decimal Calcular(SolicitudCopago solicitud) => solicitud.Paciente.EsPrimeraVez ? 20.0m : 0m;
}

public sealed class RecargoRadiografia : IRecargoCopago
{
    public decimal Calcular(SolicitudCopago solicitud) => solicitud.RequiereRadiografia ? 35.0m : 0m;
}