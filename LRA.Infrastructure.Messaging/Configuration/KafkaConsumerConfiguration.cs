namespace LRA.Infrastructure.Messaging.Configuration;

public class KafkaConsumerConfiguration
{
    public required string BootstrapServers { get; set; }
    public required string Topic { get; set; }
    public required string GroupId { get; set; }
}
