using ApprovalSystem.Domain;
using ApprovalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Application
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        UserRole Role { get; }
    }

    public interface IAccessRequestRepository
    {
        Task<AccessRequest?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsPendingAsync(Guid requesterId, Guid documentId, AccessType access, CancellationToken ct);
        Task AddAsync(AccessRequest entity, CancellationToken ct);

        Task<IReadOnlyList<AccessRequest>> ListMineAsync(Guid requesterId, RequestFilterStatus status, CancellationToken ct);
        Task<IReadOnlyList<AccessRequest>> ListAllAsync(RequestFilterStatus status, CancellationToken ct);
    }

    public interface IUserReadService
    {
        Task<bool> ExistsAsync(Guid userId, CancellationToken ct);
    }

    public interface IDocumentReadService
    {
        Task<bool> ExistsAsync(Guid documentId, CancellationToken ct);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
    }

    public interface INotificationBus
    {
        // simple log
        Task PublishAsync(string message, CancellationToken ct);

        // contract for outbox/bus
        Task PublishRawAsync(string type, string payload, Guid correlationId, CancellationToken ct);
    }

    public interface IOutbox
    {
        Task EnqueueAsync(string type, string payload, DateTime occurredUtc, CancellationToken ct);
    }
}
