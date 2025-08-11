using ApprovalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Infrastructure;
public class ApprovalSystemDbContext : DbContext
{
    public ApprovalSystemDbContext(DbContextOptions<ApprovalSystemDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AccessRequest
        modelBuilder.Entity<AccessRequest>(b =>
        {
            b.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            b.HasOne(x => x.Decision)
             .WithOne()
             .HasForeignKey<Decision>(d => d.AccessRequestId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.RequesterId, x.DocumentId, x.Status });
        });

        // Decision
        modelBuilder.Entity<Decision>(b =>
        {
            b.Property(x => x.Comment).HasMaxLength(500);
            b.HasIndex(x => x.AccessRequestId).IsUnique(); // ensure 1:1
        });

        // Users
        modelBuilder.Entity<User>(b =>
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        // Documents
        modelBuilder.Entity<Document>(b =>
        {
            b.Property(x => x.Title).IsRequired().HasMaxLength(500);
        });
    }
}
