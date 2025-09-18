using System.Net;
using System.Net.Mail;
using LRA.Common.Models;
using LRA.Email.Configuration;
using LRA.Email.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Email.Implementations;

public class SmtpService : ISmtpService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<SmtpService> _logger;
    SmtpClient _smtpClient;

    public SmtpService(IOptions<SmtpSettings> smtpSettings, ILogger<SmtpService> logger)
    {
        _logger = logger;
        _smtpSettings = smtpSettings.Value;   
        _smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl
        };
        _logger.LogInformation("SMTP client initialized for host {Host}:{Port} with SSL {SslEnabled}", 
            _smtpSettings.Host, 
            _smtpSettings.Port, 
            _smtpSettings.EnableSsl);
    }
    
    public async Task SendEmailAsync(EmailСontent content, string toEmail, CancellationToken cancellationToken)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail),
            Subject = content.Subject,
            Body = content.Body,
            IsBodyHtml = false
        };
        mailMessage.To.Add(toEmail);
        _logger.LogInformation(
            "Email successfully sent. To: {To}, Subject: {Subject}, Message ID: {MessageId}",
            toEmail,
            content.Subject,
            mailMessage.Headers["Message-ID"]);
        await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
