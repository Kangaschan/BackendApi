using Confluent.Kafka;

namespace LRA.Infrastructure.Messaging.Interfaces;

public interface IKafkaConsumer
{
    void Subscribe();
    TMessage Consume<TMessage>(CancellationToken cancellationToken);
    void Close();
}
