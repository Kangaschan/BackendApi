using System.Text.Json;
using Confluent.Kafka;

namespace LRA.Infrastructure.Messaging.Implementations;

public class KafkaJsonDeserializer<TMessage> : IDeserializer<TMessage>
{
    public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<TMessage>(data);
    }
}
