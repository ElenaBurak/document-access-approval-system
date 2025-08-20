using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Domain.Events
{
    public record DecisionMade(
    Guid AccessRequestId,     
    Guid ApproverId,           
    DecisionType Type,         // Approve/Reject
    DateTime DecidedAtUtc,     
    string? Comment,           
    string CorrelationId       // to trace requests through services
);
}
