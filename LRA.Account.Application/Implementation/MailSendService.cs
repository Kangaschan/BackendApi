using LRA.Account.Application.Interfaces;
using LRA.Common.Models;
using LRA.Infrastructure.Messaging.Interfaces;

namespace LRA.Account.Application.Implementation;

public class MailSendService : IMailSendService
{
    private readonly IKafkaProducer _producer;

    public MailSendService(IKafkaProducer producer)
    {
        _producer = producer;
    }
    
    public async Task SendMailAsync(EmailMessage mailMessage, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(mailMessage, cancellationToken);
    }
}
