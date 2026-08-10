using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected BaseController() { }

    protected IActionResult HandleResult<T>(ResponseBase<T> response, int failureStatusCode = StatusCodes.Status400BadRequest) where T : class
    {
        if (response.IsSuccess)
            return Ok(response);

        return StatusCode(failureStatusCode, response);
    }
    protected IActionResult HandleResult(ResponseBase response, int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        if (response.IsSuccess)
            return Ok(response);

        return StatusCode(failureStatusCode, response);
    }
}
