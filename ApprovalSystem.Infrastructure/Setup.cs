using ApprovalSystem.Application;
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
            services.AddScoped<IAccessRequestRepository, EfAccessRequestRepository>();
            services.AddScoped<IUserReadService, EfUserReadService>();
            services.AddScoped<IDocumentReadService, EfDocumentReadService>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            //services.AddSingleton<INotificationBus, NoopNotificationBus>();
            return services;
        }
    }
}
