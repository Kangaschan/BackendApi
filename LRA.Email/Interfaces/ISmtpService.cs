using LRA.Common.Models;

namespace LRA.Email.Interfaces;

public interface ISmtpService
{
    Task SendEmailAsync(EmailСontent content, string toEmail, CancellationToken cancelationToken = default);
}
