using NetArchTest.Rules;
using Xunit;

namespace ApprovalSystem.Test.ArchitectureTests
{
    public class ArchitectureTests
    {
        [Fact]
        public void Domain_should_depend_on_nobody()
        {
            var domain = Types.InAssembly(typeof(ApprovalSystem.Domain.Entities.AccessRequest).Assembly);

            var result = domain
                .ShouldNot()
                .HaveDependencyOnAny(
                    "ApprovalSystem.Application",
                    "ApprovalSystem.Infrastructure",
                    "Microsoft.EntityFrameworkCore")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Application_should_not_depend_on_Infrastructure()
        {
            var app = Types.InAssembly(typeof(ApprovalSystem.Application.RequestService).Assembly);

            var result = app
                .ShouldNot()
                .HaveDependencyOnAny(
                    "ApprovalSystem.Infrastructure",
                    "Microsoft.EntityFrameworkCore")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Infrastructure_should_not_depend_on_Api()
        {
            var infra = Types.InAssembly(typeof(ApprovalSystem.Infrastructure.ApprovalSystemDbContext).Assembly);

            var result = infra
                .ShouldNot()
                .HaveDependencyOnAny("ApprovalSystem.Api")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }
}
