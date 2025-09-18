namespace LRA.Infrastructure.Messaging.Configuration;

public class KafkaProducerConfiguration
{
    public required string BootstrapServers { get; set; }
    public required string Topic { get; set; }
}
