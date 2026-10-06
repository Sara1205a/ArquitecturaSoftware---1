using DentaCare.Refactored.Aplicacion;
using DentaCare.Refactored.Dominio.Politicas;
using DentaCare.Refactored.Infraestructura.Configuracion;
using DentaCare.Refactored.Infraestructura.Estadisticas;
using DentaCare.Refactored.Infraestructura.Notificaciones;
using DentaCare.Refactored.Infraestructura.Persistencia;

namespace DentaCare.Refactored;

public static class ComposicionRaiz
{
    public static ServiciosClinica Construir(bool modoProduccion)
    {
        ICalculadoraCopago calculadoraCopago = new CalculadoraCopago(
            new IReglaTarifaEspecialidad[]
            {
                new TarifaOrtodoncia(),
                new TarifaEndodoncia(),
                new TarifaCirugia(),
                new TarifaOdontopediatria()
            },
            new IDescuentoConvenio[]
            {
                new DescuentoParticular(),
                new DescuentoEps(),
                new DescuentoPrepagada()
            },
            new IRecargoCopago[]
            {
                new RecargoPrimeraVez(),
                new RecargoRadiografia()
            });

        ICalculadoraPenalizacion calculadoraPenalizacion = new CalculadoraPenalizacion(
            new IReglaPenalizacion[]
            {
                new PenalizacionCancelacionTardia(),
                new PenalizacionAdicionalCirugia()
            });

        IRepositorioCitasEscritura repositorioEscritura;
        IRepositorioCitasLectura repositorioLectura;
        IEnumerable<ICanalNotificacion> canales;

        if (modoProduccion)
        {
            var sql = new SqlServerRepositorioCitas(new OpcionesSqlServer(VariableRequerida("DENTACARE_SQL")));
            repositorioEscritura = sql;
            repositorioLectura = sql;

            string servidorSmtp = Environment.GetEnvironmentVariable("DENTACARE_SMTP") ?? "smtp.dentacare.com";
            canales = new ICanalNotificacion[]
            {
                new CanalCorreoSmtp(new OpcionesSmtp(servidorSmtp, 25, "citas@dentacare.com")),
                new CanalSmsTwilio(new OpcionesTwilio(VariableRequerida("TWILIO_API_KEY")))
            };
        }
        else
        {
            var memoria = new RepositorioCitasEnMemoria();
            repositorioEscritura = memoria;
            repositorioLectura = memoria;

            canales = new ICanalNotificacion[]
            {
                new CanalConsola("CORREO"),
                new CanalConsola("SMS")
            };
        }

        INotificadorCitas notificador = new NotificadorCitas(canales);
        var estadisticas = new EstadisticasEnMemoria();

        return new ServiciosClinica(
            new AgendarCitaServicio(new ValidadorDisponibilidadOdontologo(), calculadoraCopago,
                                    repositorioEscritura, notificador, estadisticas),
            new CancelarCitaServicio(calculadoraPenalizacion, repositorioEscritura, notificador, estadisticas),
            new ConsultarCitasServicio(repositorioLectura),
            estadisticas);
    }

    private static string VariableRequerida(string nombre)
    {
        return Environment.GetEnvironmentVariable(nombre)
            ?? throw new InvalidOperationException($"Falta la variable de entorno {nombre}.");
    }
}