
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : BaseController
{
    private readonly IMediator _mediator;

    public HealthCheckController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthCheck() => Ok();
}
