namespace App.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class BaseController : ControllerBase
{
    protected IMediator Mediator =>  HttpContext.RequestServices.GetRequiredService<IMediator>();
}