using ApprovalSystem.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Infrastructure.Repositories;

public class EfDocumentReadService(ApprovalSystemDbContext db) : IDocumentReadService
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct) => db.Documents.AnyAsync(d => d.Id == id, ct);
}