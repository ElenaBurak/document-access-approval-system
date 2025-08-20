using ApprovalSystem.Application;
using ApprovalSystem.Infrastructure.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Infrastructure.Repositories
{
    internal class OutboxRepository : IOutbox
    {
        private readonly ApprovalSystemDbContext _db;
        public OutboxRepository(ApprovalSystemDbContext db) => _db = db;

        public Task EnqueueAsync(string type, string payload, DateTime occurredUtc, CancellationToken ct)
        {
            _db.Outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                CorrelationId = Guid.NewGuid(),
                OccurredUtc = occurredUtc
            });
            return Task.CompletedTask;
        }
    }
}
