using ApprovalSystem.Domain.Entities;
using ApprovalSystem.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Infrastructure;
public class ApprovalSystemDbContext : DbContext
{
    public ApprovalSystemDbContext(DbContextOptions<ApprovalSystemDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AccessRequest
        modelBuilder.Entity<AccessRequest>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            b.HasIndex(x => new { x.RequesterId, x.DocumentId, x.Status });
            b.HasOne(x => x.Decision)
             .WithOne(d => d.AccessRequest)
             .HasForeignKey<Decision>(d => d.AccessRequestId)            
             .OnDelete(DeleteBehavior.Cascade);

        });

        // Decision
        modelBuilder.Entity<Decision>(b =>
        {
            b.HasKey(x => x.AccessRequestId);
            b.Property(x => x.AccessRequestId).ValueGeneratedNever(); 
            b.Property(x => x.Comment).HasMaxLength(500);           
        });

        // Users
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        // Documents
        modelBuilder.Entity<Document>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Type).IsRequired().HasMaxLength(200);
            b.Property(x => x.Payload).IsRequired();            
        });
    }
}
