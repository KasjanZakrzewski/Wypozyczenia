using MailKit.Security;
using MimeKit;
namespace WypozyczeniaAPI.Services
{
    public interface IEmailService
    {
        // Metoda służąca obsługi wysyłania email 
        void SendEmail(string toAddress, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        // Metoda służąca obsługi wysyłania email 
        public void SendEmail(string toAddress, string subject, string body)
        {
            // Tymczasowe obejście,
            // docelowo nie będzie napisywania 
            //toAddress = "mail tymczosowy";

            // Generujemy treść maila
            var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("WypożyczenieAPI", "@gmail.com"));
            message.To.Add(new MailboxAddress("Odbiorca", toAddress));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            // wysyłamy wygenreowany mail
            // za pomocą smtp.gmail.com
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("", "");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}