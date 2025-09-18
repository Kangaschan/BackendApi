using Confluent.Kafka;
using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Infrastructure.Messaging.Implementations;

public class KafkaProducer : IKafkaProducer
{
    private readonly KafkaProducerConfiguration _producerConfiguration;
    private readonly IProducer<string, object> _producer;

    public KafkaProducer(IOptions<KafkaProducerConfiguration> configuration)
    {
        _producerConfiguration = configuration.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = _producerConfiguration.BootstrapServers
        };
        
        _producer = new ProducerBuilder<string, object>(config)
            .SetValueSerializer(new KafkaJsonSerializer<object>())
            .Build();
    }

    public async Task ProduceAsync(object message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(_producerConfiguration.Topic, new Message<string, object>
        {
            Key = "uniq1",
            Value = message
        }, cancellationToken);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
