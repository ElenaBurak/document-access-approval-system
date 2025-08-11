using ApprovalSystem.Domain;
using ApprovalSystem.Domain.Entities;

namespace ApprovalSystem.Application;
public sealed class RequestService
{
    private readonly ICurrentUser _current;
    private readonly IAccessRequestRepository _repo;
    private readonly IDocumentReadService _docs;
    private readonly IUserReadService _users;
    private readonly IUnitOfWork _uow;
    private readonly INotificationBus _bus;

    public RequestService(
        ICurrentUser current,
        IAccessRequestRepository repo,
        IDocumentReadService docs,
        IUserReadService users,
        IUnitOfWork uow,
        INotificationBus bus)
    {
        _current = current;
        _repo = repo;
        _docs = docs;
        _users = users;
        _uow = uow;
        _bus = bus;
    }

    // 1) Create access request
    public async Task<AccessRequestDetailsDto> CreateAsync(CreateAccessRequestCommand cmd, CancellationToken ct)
    {
        if (cmd.DocumentId == Guid.Empty) throw Errors.BadRequest("DocumentId is required");
        if (string.IsNullOrWhiteSpace(cmd.Reason)) throw Errors.BadRequest("Reason is required");
        if (!await _docs.ExistsAsync(cmd.DocumentId, ct)) throw Errors.NotFound(nameof(Document), cmd.DocumentId);
        if (!await _users.ExistsAsync(_current.UserId, ct)) throw Errors.Forbidden("Current user not found");

        // Optional business rule: prevent duplicate pending of same type
        if (await _repo.ExistsPendingAsync(_current.UserId, cmd.DocumentId, cmd.RequestedAccess, ct))
            throw Errors.Conflict("Pending request of the same type already exists for this document");

        var entity = AccessRequest.Create(cmd.DocumentId, _current.UserId, cmd.RequestedAccess, cmd.Reason);
        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return MapDetails(entity);
    }

    // 2) List
    public async Task<IReadOnlyList<AccessRequestSummaryDto>> ListAsync(bool mineOnly, RequestFilterStatus status, CancellationToken ct)
    {
        IReadOnlyList<AccessRequest> items;
        if (mineOnly || _current.Role == UserRole.User)
        {
            items = await _repo.ListMineAsync(_current.UserId, status, ct);
        }
        else
        {
            // Approver/Admin can see all
            items = await _repo.ListAllAsync(status, ct);
        }
        return items.Select(MapSummary).ToArray();
    }

    // 3) Get by id
    public async Task<AccessRequestDetailsDto> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct) ?? throw Errors.NotFound(nameof(AccessRequest), id);

        if (_current.Role == UserRole.User && entity.RequesterId != _current.UserId)
            throw Errors.Forbidden("You can only view your own requests");

        return MapDetails(entity);
    }

    // 4) Make decision
    public async Task<AccessRequestDetailsDto> DecideAsync(MakeDecisionCommand cmd, CancellationToken ct)
    {
        if (_current.Role is not (UserRole.Approver or UserRole.Admin))
            throw Errors.Forbidden("Only approvers can decide requests");

        var entity = await _repo.GetByIdAsync(cmd.AccessRequestId, ct)
                     ?? throw Errors.NotFound(nameof(AccessRequest), cmd.AccessRequestId);

        Decision decision = cmd.Decision == DecisionType.Approve
            ? entity.Approve(_current.UserId, cmd.Comment)
            : entity.Reject(_current.UserId, cmd.Comment);

        await _uow.SaveChangesAsync(ct);

        // fire-and-forget notification (demo)
        try
        {
            await _bus.PublishAsync($"Request {entity.Id} {cmd.Decision} by {_current.UserId}", ct);
        }
        catch { /* swallow in demo */ }

        return MapDetails(entity);
    }
    
    // Mappers    
    private static AccessRequestSummaryDto MapSummary(AccessRequest ar) => new(
        ar.Id,
        ar.DocumentId,
        ar.RequestedAccess,
        ar.Status,
        ar.CreatedAtUtc
    );

    private static AccessRequestDetailsDto MapDetails(AccessRequest ar)
    {
        DecisionDto? d = null;
        if (ar.Decision is not null)
        {
            d = new DecisionDto(
                ar.Decision.Type,
                ar.Decision.ApproverId,
                ar.Decision.Comment,
                ar.Decision.CreatedAtUtc
            );
        }

        return new AccessRequestDetailsDto(
            ar.Id,
            ar.DocumentId,
            ar.RequesterId,
            ar.RequestedAccess,
            ar.Reason,
            ar.Status,
            ar.CreatedAtUtc,
            ar.DecidedAtUtc,
            d
        );
    }
}