using Microsoft.AspNetCore.Identity.UI.Services;

namespace CuaHangBangDiaNhac.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"--- EMAIL GỬI TỚI: {email} ---");
            _logger.LogInformation($"--- TIÊU ĐỀ: {subject} ---");
            _logger.LogInformation($"--- NỘI DUNG: \n{htmlMessage}");
            _logger.LogInformation("---------------------------------");

            return Task.CompletedTask;
        }
    }
}