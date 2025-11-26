using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartBookDef.Application.Options;

namespace SmartBookDef.Application.Services;

public class EmailService
{
    private readonly Correo _correo;


    public EmailService(IOptions<Correo> options)
    {
        _correo = options.Value;
    }

    public void Enviar(string destinatario, string asunto, string mensaje)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("SmartBookU", _correo.cuenta));
        email.To.Add(new MailboxAddress("", destinatario));
        email.Subject = asunto;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mensaje
        };

        email.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", _correo.host, MailKit.Security.SecureSocketOptions.StartTls);

            client.Authenticate(_correo.cuenta, _correo.contrasenia);

            client.Send(email);
            client.Disconnect(true);
        }
    }
}
