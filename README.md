# Document Access Approval System

## Quick start
```bash
dotnet build
dotnet run --project ApprovalSystem.Api
```

**Seed:**  
User `00000000-0000-0000-0000-000000000001` • Approver `00000000-0000-0000-0000-000000000002` • Document `00000000-0000-0000-0000-000000000010`

## Postman (in solution)
- Collection & environment are in `docs/postman/` inside the solution.  
- Import both, set `BaseUrl`, run flow: **Create → List Pending → Decide → Get by Id**.  
- Required headers: `X-User-Id`, `X-Role` (`User`/`Approver`).  
- Route base: `/api/requests`.
- Note: InMemory DB resets on app restart.

## Structure
- **API** — controllers, Swagger, global error handler.
- **Application** — use cases & DTOs (`RequestService`, `IAccessRequestRepository`, `ICurrentUser`, `INotificationBus`).
- **Domain** — `AccessRequest`, `Decision`, `User`, `Document`, enums.
- **Infrastructure** — EF Core InMemory, repositories, seeding, `NoopNotificationBus`.

## Design notes (assumptions & trade-offs)
- Simplified auth: roles come from headers via `HeaderCurrentUser`.
- InMemory storage for zero setup (resets on app restart).
- DTOs decouple API from domain; minimal validation via data annotations.

## If more time
Real auth (JWT/policies), SQLite + migrations, CQRS + FluentValidation, background notifications (queue/email) with outbox, pagination/sorting, Dockerfile & CI, tests (xUnit, Moq), Swagger improvements, more complex domain logic.
