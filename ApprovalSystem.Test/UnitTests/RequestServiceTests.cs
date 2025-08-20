using ApprovalSystem.Application;
using ApprovalSystem.Domain;
using ApprovalSystem.Domain.Entities;
using Moq;
using Xunit;

namespace ApprovalSystem.Test.UnitTests
{

    public class RequestServiceTests
    {
        [Fact]
        public async Task CreateAsync_ValidInput_AddsRequest_AndSaves()
        {
            // arrange
            var repo = new Mock<IAccessRequestRepository>();
            var outbox = new Mock<IOutbox>();
            var docs = new Mock<IDocumentReadService>();
            var users = new Mock<IUserReadService>();
            var uow = new Mock<IUnitOfWork>();
            var bus = new Mock<INotificationBus>();
            var current = new Mock<ICurrentUser>();

            current.Setup(c => c.UserId).Returns(Guid.NewGuid());
            var docId = Guid.NewGuid();

            docs.Setup(d => d.ExistsAsync(docId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            users.Setup(u => u.ExistsAsync(current.Object.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(r => r.ExistsPendingAsync(current.Object.UserId, docId, AccessType.Read, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            AccessRequest? captured = null;
            repo.Setup(r => r.AddAsync(It.IsAny<AccessRequest>(), It.IsAny<CancellationToken>()))
                .Callback<AccessRequest, CancellationToken>((ar, _) => captured = ar)
                .Returns(Task.CompletedTask);

            uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var svc = new RequestService(current.Object, repo.Object, outbox.Object, docs.Object, users.Object, uow.Object, bus.Object);

            var cmd = new CreateAccessRequestCommand(docId, "Need read", AccessType.Read);

            // act
            var result = await svc.CreateAsync(cmd, CancellationToken.None);

            // assert (data)
            Assert.Equal(RequestStatus.Pending, result.Status);
            Assert.Equal(docId,result.DocumentId);
            Assert.Equal(current.Object.UserId, result.RequesterId);
            Assert.Equal(AccessType.Read, result.RequestedAccess);
            Assert.Equal("Need read", result.Reason);

            // assert (interactions)
            Assert.NotNull(captured);
            Assert.Equal(current.Object.UserId, captured!.RequesterId);
            repo.Verify(r => r.AddAsync(It.IsAny<AccessRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            // bus is not called on Create
            bus.Verify(b => b.PublishAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
