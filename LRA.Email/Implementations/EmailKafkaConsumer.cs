using System.Text.Json;
using LRA.Common.Models;
using LRA.Email.Interfaces;
using LRA.Infrastructure.Messaging.Interfaces;

namespace LRA.Email.Implementations;

public class EmailKafkaConsumer : BackgroundService
{
    private readonly ISmtpService _smtpService;
    private readonly ILogger<EmailKafkaConsumer> _logger;
    private readonly IKafkaConsumer _kafkaConsumer;
    
    public EmailKafkaConsumer(
        ISmtpService smtpService,
        ILogger<EmailKafkaConsumer> logger,
        IKafkaConsumer kafkaConsumer)
    {
        _smtpService = smtpService;
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Subscribe();
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _kafkaConsumer.Consume<EmailMessage>(cancellationToken);
                _logger.LogInformation("Received Kafka message: {MessageType} with length {MessageLength}", result.GetType().Name, result.ToString().Length);
                await ProcessMessageAsync(result, cancellationToken);
                await Task.Delay(200, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Kafka consumer. Delay {DelayMs}ms before retry",1000);
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
    
    private async Task ProcessMessageAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing email to {Recipient} with body length {BodyLength}", message.RecipientEmail, message.Content.Body.Length);
            await _smtpService.SendEmailAsync(message.Content, message.RecipientEmail, cancellationToken);
        }
        catch(InvalidCastException  ex)
        {
            _logger.LogWarning(ex, "Received unexpected message type {MessageType}: {MessageContent}", message?.GetType().Name, message);
        }
    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Close();
        return base.StopAsync(cancellationToken);
    }
}
