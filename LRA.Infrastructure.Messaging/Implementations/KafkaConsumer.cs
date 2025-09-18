using System.Text.Json;
using Confluent.Kafka;
using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Infrastructure.Messaging.Implementations;

public class KafkaConsumerBase : IKafkaConsumer
{
    private readonly KafkaConsumerConfiguration _consumerConfiguration;
    private readonly IConsumer<string, object> _consumer;

    public KafkaConsumerBase(IOptions<KafkaConsumerConfiguration> configuration)
    {
        _consumerConfiguration = configuration.Value;
        var config = new ConsumerConfig
        {
            BootstrapServers = _consumerConfiguration.BootstrapServers,
            GroupId = _consumerConfiguration.GroupId,
        };
        
        _consumer = new ConsumerBuilder<string, object>(config)
            .SetValueDeserializer(new KafkaJsonDeserializer<object>())
            .Build();
    }   

    public void Subscribe()
    {
        _consumer.Subscribe(_consumerConfiguration.Topic);
    }

    public TMessage Consume<TMessage>(CancellationToken cancellationToken)
    {
        var consumedMessage  =_consumer.Consume(cancellationToken);
        var result =  JsonSerializer.Deserialize<TMessage>((JsonElement)consumedMessage.Message.Value);
        
        return result;
    }

    public void Close()
    {
        _consumer.Close();
    }
}
