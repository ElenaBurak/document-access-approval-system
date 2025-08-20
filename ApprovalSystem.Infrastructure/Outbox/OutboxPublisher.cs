using ApprovalSystem.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApprovalSystem.Infrastructure.Outbox;
/// <summary>
/// Reads messages from the Outbox table and publishes them to the bus.
/// </summary>
public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxPublisher> _log;

    public OutboxPublisher(IServiceProvider sp, ILogger<OutboxPublisher> log)
    {
        _sp = sp; _log = log;
    }

    /// <summary>
    /// The host calls it when the application starts.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // spin while the app is alive
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApprovalSystemDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<INotificationBus>();

                var batch = await db.Outbox
                    .Where(x => x.SentUtc == null && x.Attempt < 5)
                    .OrderBy(x => x.OccurredUtc)
                    .Take(50)
                    .ToListAsync(ct);

                foreach (var m in batch)
                {
                    try
                    {
                        await bus.PublishRawAsync(m.Type, m.Payload, m.CorrelationId, ct);
                        m.SentUtc = DateTime.UtcNow;
                        m.Error = null;
                        _log.LogInformation("Outbox sent: {Id} {Type}", m.Id, m.Type);
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        m.Attempt++;
                        m.Error = ex.Message;
                        _log.LogWarning(ex, "Outbox publish failed (attempt {Attempt}) for {Id}", m.Attempt, m.Id);
                    }
                }

                await db.SaveChangesAsync(ct);

                // If there is nothing to send, let's wait a little longer
                var delayMs = batch.Count == 0 ? 2000 : 200;
                await Task.Delay(delayMs, ct);
            }
            catch (OperationCanceledException)
            {
                // stop app
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "OutboxPublisher loop error");
                await Task.Delay(2000, ct);
            }
        }
    }
}

