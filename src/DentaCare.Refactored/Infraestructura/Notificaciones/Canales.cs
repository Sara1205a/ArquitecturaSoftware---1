using System.Net.Mail;
using DentaCare.Refactored.Aplicacion;
using DentaCare.Refactored.Dominio.Modelo;
using DentaCare.Refactored.Infraestructura.Configuracion;

namespace DentaCare.Refactored.Infraestructura.Notificaciones;

public sealed class CanalCorreoSmtp : ICanalNotificacion
{
    private readonly OpcionesSmtp _opciones;

    public CanalCorreoSmtp(OpcionesSmtp opciones)
    {
        _opciones = opciones;
    }

    public string Nombre => "Correo SMTP";

    public void Enviar(Paciente destinatario, string asunto, string mensaje)
    {
        using var cliente = new SmtpClient(_opciones.Servidor, _opciones.Puerto);
        using var correo = new MailMessage(_opciones.Remitente, destinatario.Correo, asunto, mensaje);
        cliente.Send(correo);
    }
}

public sealed class CanalSmsTwilio : ICanalNotificacion
{
    private readonly OpcionesTwilio _opciones;

    public CanalSmsTwilio(OpcionesTwilio opciones)
    {
        _opciones = opciones;
    }

    public string Nombre => "SMS Twilio";

    public void Enviar(Paciente destinatario, string asunto, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(_opciones.ApiKey))
        {
            throw new InvalidOperationException("La credencial de Twilio no está configurada.");
        }

        // Simulado, igual que en el legado. Ya NO se imprime la API Key.
        Console.WriteLine($"   [SMS simulado vía Twilio -> {destinatario.Celular}]: {mensaje}");
    }
}

public sealed class CanalConsola : ICanalNotificacion
{
    private readonly string _nombre;

    public CanalConsola(string nombre)
    {
        _nombre = nombre;
    }

    public string Nombre => _nombre;

    public void Enviar(Paciente destinatario, string asunto, string mensaje)
    {
        Console.WriteLine($"   [{_nombre} simulado -> {destinatario.NombreCompleto}] {asunto}: {mensaje}");
    }
}