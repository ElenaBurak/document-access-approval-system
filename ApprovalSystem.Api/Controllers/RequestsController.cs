using ApprovalSystem.Application;
using Microsoft.AspNetCore.Mvc;

namespace DocumentAccessApprovalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly RequestService _service;

        public RequestsController(RequestService service) => _service = service;

        // POST /api/access-requests
        [HttpPost]
        public async Task<ActionResult<AccessRequestDetailsDto>> Create([FromBody] CreateAccessRequestCommand body, CancellationToken ct)
        {
            var result = await _service.CreateAsync(body, ct);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // GET /api/access-requests?mine=true&status=Pending
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AccessRequestSummaryDto>>> List([FromQuery] bool mine = true, [FromQuery] RequestFilterStatus status = RequestFilterStatus.Any, CancellationToken ct = default)
            => Ok(await _service.ListAsync(mine, status, ct));

        // GET /api/access-requests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AccessRequestDetailsDto>> Get([FromRoute] Guid id, CancellationToken ct)
            => Ok(await _service.GetAsync(id, ct));

        // POST /api/access-requests/{id}/decisions
        [HttpPost("{id}/decisions")]
        public async Task<ActionResult<AccessRequestDetailsDto>> Decide([FromRoute] Guid id, [FromBody] MakeDecisionCommand body, CancellationToken ct)
        {
            if (id != body.AccessRequestId) return BadRequest(new ProblemDetails { Title = "Mismatched ids" });
            var result = await _service.DecideAsync(body, ct);
            return Ok(result);
        }
    }
}
