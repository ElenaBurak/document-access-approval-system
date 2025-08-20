namespace ApprovalSystem.Domain.Entities
{
    public class Decision
    {
        // PK = FK на AccessRequest
        public Guid AccessRequestId { get; set; }
        public AccessRequest AccessRequest { get; private set; } = null!; // navigation property, required for EF
        public Guid ApproverId { get; set; }
        public DecisionType Type { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
