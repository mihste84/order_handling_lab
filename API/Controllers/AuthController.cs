using Microsoft.AspNetCore.Authentication;


namespace API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppLogic.Common.Interfaces.IAuthenticationService _authenticationService;

    public AuthController(IConfiguration config, AppLogic.Common.Interfaces.IAuthenticationService authenticationService)
    {
        _config = config;
        _authenticationService = authenticationService;
    }

    public IActionResult Login([FromQuery]string returnUrl = "/")
    {
        return new ChallengeResult("OpenIdConnect", new AuthenticationProperties() { RedirectUri = returnUrl });
    }

    [Authorize]
    public async Task<IActionResult> Logout([FromQuery]string returnUrl = "/") {
        await HttpContext.SignOutAsync();
        return new SignOutResult("OpenIdConnect", new AuthenticationProperties { RedirectUri = returnUrl });
    }

    public AppUser GetAppUser() => _authenticationService.GetAppUser();
}