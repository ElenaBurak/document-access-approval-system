using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Domain.Entities
{
    public class AccessRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequesterId { get; set; }
        public Guid DocumentId { get; set; }
        public AccessType RequestedAccess { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? DecidedAtUtc { get; private set; }
        public Decision? Decision { get; private set; }

        public static AccessRequest Create(Guid documentId, Guid requesterId, AccessType access, string reason)
        {
            if (documentId == Guid.Empty) throw new ArgumentException("DocumentId is required");
            if (requesterId == Guid.Empty) throw new ArgumentException("RequesterId is required");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required");            

            return new AccessRequest
            {
                DocumentId = documentId,
                RequesterId = requesterId,
                RequestedAccess = access,
                Reason = reason.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };
        }

        public Decision Approve(Guid approverId, string? comment = null)
            => Decide(approverId, DecisionType.Approve, comment);

        public Decision Reject(Guid approverId, string? comment = null)
            => Decide(approverId, DecisionType.Reject, comment);

        private Decision Decide(Guid approverId, DecisionType type, string? comment)
        {
            if (approverId == Guid.Empty) throw new ArgumentException("ApproverId is required");
            if (Status != RequestStatus.Pending) throw new InvalidOperationException($"Request already {Status}.");
            if (approverId == RequesterId) throw new InvalidOperationException("Requester cannot decide their own request.");           

            Status = type == DecisionType.Approve ? RequestStatus.Approved : RequestStatus.Rejected;
            DecidedAtUtc = DateTime.UtcNow;

            var decision = new Decision
            {
                AccessRequestId = Id,
                ApproverId = approverId,
                Type = type,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                CreatedAtUtc = DecidedAtUtc.Value
            };

            Decision = decision;
            return decision;
        }

    }
}
