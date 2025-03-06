
namespace DoaMais.HospitalNotificationService.Services.Interface
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(string to, string subject, string templatePath, Dictionary<string, string> placeholders);
    }
}
