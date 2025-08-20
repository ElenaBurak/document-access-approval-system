
using ApprovalSystem.Application;
using Microsoft.Extensions.Logging;

namespace ApprovalSystem.Infrastructure.Notifications;

public sealed class NoopNotificationBus : INotificationBus
{
    private readonly ILogger<NoopNotificationBus> _logger;

    public NoopNotificationBus(ILogger<NoopNotificationBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[NOTIFY] {Message}", message);
        return Task.CompletedTask;
    }

    public Task PublishRawAsync(string type, string payload, Guid correlationId, CancellationToken ct)
    {
        _logger.LogInformation("[BUS] Type={Type} CorrelationId={CorrelationId} Payload={Payload}",
                              type, correlationId, payload);
        return Task.CompletedTask;
    }
}
