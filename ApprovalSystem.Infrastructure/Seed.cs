using ApprovalSystem.Domain;
using ApprovalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApprovalSystem.Infrastructure
{
    public static class Seed
    {
        public static async Task SeedAsync(this IServiceProvider services, CancellationToken ct = default)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApprovalSystemDbContext>();

            // Ensure the in-memory store is created
            await db.Database.EnsureCreatedAsync(ct);

            if (!await db.Users.AnyAsync(ct))
            {
                db.Users.AddRange(
                     new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Tom", Role = UserRole.User },
                     new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Alex", Role = UserRole.Approver },
                     new User { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Admin", Role = UserRole.Admin }
 );
            }
            if (!await db.Documents.AnyAsync(ct))
            {
                db.Documents.AddRange(
                        new Document { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Title = "Doc A" },
                        new Document { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Title = "Doc B" }
                );
            }
            await db.SaveChangesAsync(ct);
        }
    }
}
