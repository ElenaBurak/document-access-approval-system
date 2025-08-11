namespace ApprovalSystem.Domain.Entities
{
    public class Decision
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccessRequestId { get; set; }
        public Guid ApproverId { get; set; }
        public DecisionType Type { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
