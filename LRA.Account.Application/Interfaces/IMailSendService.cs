using System.Net.Mail;
using LRA.Common.Models;

namespace LRA.Account.Application.Interfaces;

public interface IMailSendService
{
    public Task SendMailAsync(EmailMessage mailMessage, CancellationToken cancellationToken);    
}
