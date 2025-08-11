using ApprovalSystem.Application;
using ApprovalSystem.Domain;
using Microsoft.AspNetCore.Http;

namespace ApprovalSystem.Infrastructure.Identity
{
    public sealed class HeaderCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var headers = _httpContextAccessor.HttpContext?.Request.Headers;
                if (headers != null && headers.TryGetValue("X-User-Id", out var userIdStr) &&
                    Guid.TryParse(userIdStr, out var userId))
                {
                    return userId;
                }
                
                return Guid.Empty;
            }
        }

        public UserRole Role
        {
            get
            {
                var headers = _httpContextAccessor.HttpContext?.Request.Headers;
                if (headers != null && headers.TryGetValue("X-Role", out var roleStr) &&
                    Enum.TryParse<UserRole>(roleStr, true, out var role))
                {
                    return role;
                }
              
                return UserRole.User;
            }
        }
    }
}
