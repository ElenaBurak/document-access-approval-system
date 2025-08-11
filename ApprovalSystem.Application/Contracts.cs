using ApprovalSystem.Domain;

namespace ApprovalSystem.Application;

public record CreateAccessRequestCommand(
    Guid DocumentId,
    string Reason,
    AccessType RequestedAccess
);

public record MakeDecisionCommand(
    Guid AccessRequestId,
    DecisionType Decision,
    string? Comment
);

public enum RequestFilterStatus { Any = 0, Pending = 1, Approved = 2, Rejected = 3 }

public record AccessRequestSummaryDto(
    Guid Id,
    Guid DocumentId,
    AccessType RequestedAccess,
    RequestStatus Status,
    DateTime CreatedAtUtc
);

public record DecisionDto(
    DecisionType Type,
    Guid ApproverId,
    string? Comment,
    DateTime CreatedAtUtc
);

public record AccessRequestDetailsDto(
    Guid Id,
    Guid DocumentId,
    Guid RequesterId,
    AccessType RequestedAccess,
    string Reason,
    RequestStatus Status,
    DateTime CreatedAtUtc,
    DateTime? DecidedAtUtc,
    DecisionDto? Decision
);

