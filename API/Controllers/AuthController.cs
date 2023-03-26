using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    public IActionResult Login(string returnUrl = "/")
    {
        return new ChallengeResult("OpenIdConnect", new AuthenticationProperties() { RedirectUri = _config["AzureAd:RedirectUri"] });
    }

    [Authorize]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync();
        return new SignOutResult("OpenIdConnect", new AuthenticationProperties { RedirectUri = _config["AzureAd:RedirectUri"] });
    }

    public AppUser GetAppUser() => _authenticationService.GetAppUser();
}