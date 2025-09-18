using System.Text.Json;
using Confluent.Kafka;

namespace LRA.Infrastructure.Messaging.Implementations;

public class KafkaJsonSerializer<TMessage> : ISerializer<TMessage>
{
    public byte[] Serialize(TMessage data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}
