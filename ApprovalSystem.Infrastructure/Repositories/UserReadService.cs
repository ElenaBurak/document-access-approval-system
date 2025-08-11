using ApprovalSystem.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Infrastructure.Repositories
{
    public sealed class UserReadService(ApprovalSystemDbContext db) : IUserReadService
    {
        public Task<bool> ExistsAsync(Guid id, CancellationToken ct) => db.Users.AnyAsync(u => u.Id == id, ct);
    }
}
