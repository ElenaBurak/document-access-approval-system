using ApprovalSystem.Application;
using ApprovalSystem.Domain;
using ApprovalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Infrastructure.Repositories
{
    public class EfAccessRequestRepository : IAccessRequestRepository
    {
        private readonly ApprovalSystemDbContext _db;
        public EfAccessRequestRepository(ApprovalSystemDbContext db) => _db = db;

        public Task<AccessRequest?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.AccessRequests.Include(r => r.Decision).FirstOrDefaultAsync(r => r.Id == id, ct);

        public Task<bool> ExistsPendingAsync(Guid requesterId, Guid documentId, AccessType access, CancellationToken ct)
            => _db.AccessRequests.AnyAsync(r => r.RequesterId == requesterId && r.DocumentId == documentId && r.RequestedAccess == access && r.Status == RequestStatus.Pending, ct);

        public Task AddAsync(AccessRequest e, CancellationToken ct) => _db.AccessRequests.AddAsync(e, ct).AsTask();

        public async Task<IReadOnlyList<AccessRequest>> ListMineAsync(Guid requesterId, RequestFilterStatus status, CancellationToken ct)
        { var q = _db.AccessRequests.AsNoTracking().Where(r => r.RequesterId == requesterId); return await Apply(status, q).OrderByDescending(r => r.CreatedAtUtc).ToListAsync(ct); }

        public async Task<IReadOnlyList<AccessRequest>> ListAllAsync(RequestFilterStatus status, CancellationToken ct)
        { var q = Apply(status, _db.AccessRequests.AsNoTracking()); return await q.OrderByDescending(r => r.CreatedAtUtc).ToListAsync(ct); }

        static IQueryable<AccessRequest> Apply(RequestFilterStatus s, IQueryable<AccessRequest> q)
            => s switch
            {
                RequestFilterStatus.Pending => q.Where(r => r.Status == RequestStatus.Pending),
                RequestFilterStatus.Approved => q.Where(r => r.Status == RequestStatus.Approved),
                RequestFilterStatus.Rejected => q.Where(r => r.Status == RequestStatus.Rejected),
                _ => q
            };
    }
}
