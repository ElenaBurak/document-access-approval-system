using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystem.Application
{
    public static class Errors
    {
        public static Exception NotFound(string entity, Guid id) => new KeyNotFoundException($"{entity} {id} not found");
        public static Exception Forbidden(string message) => new UnauthorizedAccessException(message);
        public static Exception Conflict(string message) => new InvalidOperationException(message);
        public static Exception BadRequest(string message) => new ArgumentException(message);
    }
}
