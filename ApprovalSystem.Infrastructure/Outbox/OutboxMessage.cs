using System;

namespace ApprovalSystem.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }                    // PK
    public string Type { get; set; } = default!;    // event type name, "DecisionMade"
    public string Payload { get; set; } = default!; // JSON events 
    public DateTime OccurredUtc { get; set; }      
    public DateTime? SentUtc { get; set; }          
    public int Attempt { get; set; }                
    public string? Error { get; set; }    
    public Guid CorrelationId { get; set; }
}