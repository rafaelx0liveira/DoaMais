using DoaMais.HospitalNotificationService.Services.Interface;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Web;

namespace DoaMais.HospitalNotificationService.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly string _apiKey;

        public SendEmailService()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User) ?? throw new ArgumentNullException("SENDGRID_API_KEY not found.");
        }

        public async Task SendEmailAsync(string to, string subject, string templatePath, Dictionary<string, string> placeholders)
        {
            string body = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

            body = HttpUtility.HtmlDecode(body);

            foreach (var placeholder in placeholders)
            {
                body = body.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("rafaelaparecido.oliveirasilva@gmail.com");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                plainTextContent: null,
                htmlContent: body
            );

            var response = await client.SendEmailAsync(msg);

            // Verifica o status da resposta
            if ((int)response.StatusCode >= 400)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
