using EmailService;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Linq;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _config;

    public EmailSender(IOptions<EmailConfiguration> config)
    {
     

        _config = config.Value;

      
    }

   

    public async Task SendEmailAsync(Message message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config.From),
            Subject = message.Subject,
            Body = message.Content,
            IsBodyHtml = true
        };

        if (message.To == null || !message.To.Any())
            throw new InvalidOperationException("At least one recipient is required.");

        foreach (var recipient in message.To)
        {
            mailMessage.To.Add(recipient);
        }


        using var client = new SmtpClient
        {
            Host = _config.SmtpServer,
            Port = _config.Port,
            EnableSsl = _config.EnableSsl,
            Credentials = new NetworkCredential(_config.Username, _config.Password)
        };

        await client.SendMailAsync(mailMessage);
    }



}
