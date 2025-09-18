namespace LRA.Infrastructure.Messaging.Interfaces;

public interface IKafkaProducer : IDisposable
{
    Task ProduceAsync(object message, CancellationToken cancellationToken);
}
