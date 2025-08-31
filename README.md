# Document Access Approval System
## Objective:
Design and implement a small but well-structured REST API for managing access requests 
to internal documents. This assignment mimics a real-world enterprise scenario where 
sensitive documents require an approval workflow.

Business Requirements:

  Actors:
  - User – can request access to a document
  - Approver – reviews and accepts/rejects requests
  - (Optional) Admin – manages access
    
  Use Cases:
  
    1. A user submits an access request to a document, including:
      - Document ID
      - Reason for access
      - Requested access type (Read / Edit)
    2. The approver sees a list of pending requests and can:
      - Approve or reject with a comment
    3. The user can check the status of their requests. 

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
