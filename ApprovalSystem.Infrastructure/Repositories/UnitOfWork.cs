using ApprovalSystem.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Infrastructure.Repositories
{
    public class UnitOfWork(ApprovalSystemDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    }
}
