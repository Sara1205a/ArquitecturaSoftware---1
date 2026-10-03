using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace DentaCare.Legacy
{
    public class NotificacionServicio
    {
        private string _servidorSmtp = "smtp.dentacare.com";
        private string _twilioApiKey = "TWILIO_SECRET_KEY_998877";
        public void EnviarEmailYSms(Cita cita, string mensaje)
        {
            SmtpClient client = new SmtpClient(_servidorSmtp);
            MailMessage mail = new MailMessage("citas@dentacare.com", cita.Paciente.Correo);
            mail.Subject = "Confirmación de Cita Odontológica";
            mail.Body = $"Su cita quedó programada para el {cita.FechaHora}. Copago estimado: ${cita.CopagoCalculado}";
            client.Send(mail);

            // Simulación de envío SMS por Twilio API
            Console.WriteLine($"[SMS enviado con API Key {_twilioApiKey}]: Cita confirmada para {cita.FechaHora}");
        }
    }
}
