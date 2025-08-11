using ApprovalSystem.Application;
using ApprovalSystem.Infrastructure.Notifications;
using ApprovalSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApprovalSystem.Infrastructure
{
    public static class Setup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string databaseName = "AccessControlDb")
        {
            services.AddDbContext<ApprovalSystemDbContext>(o => o.UseInMemoryDatabase(databaseName));
            services.AddScoped<IAccessRequestRepository, AccessRequestRepository>();
            services.AddScoped<IUserReadService, UserReadService>();
            services.AddScoped<IDocumentReadService, DocumentReadService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<INotificationBus, NoopNotificationBus>();
            return services;
        }
    }
}
