using Common.Models;
using OneOf.Types;

namespace App.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class BaseController : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult Ok<T>(Success<T> success)
    {
        return Ok(success.Value);
    }

    protected IActionResult InternalServerError(Error<string> error)
    {
        return StatusCode(500, new { Message = error.Value });
    }

    protected IActionResult ValidationBadRequest(ValidationError error)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = "One or more validation errors occurred.",
            Instance = HttpContext.Request.Path
        };

        problemDetails.Extensions["errors"] = error.Errors;

        return BadRequest(problemDetails);
    }
}