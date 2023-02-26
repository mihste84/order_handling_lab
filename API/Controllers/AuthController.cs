using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]/[action]")]
public class AuthController : Controller
{
    private IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
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

    public AppUser GetAppUser() => new() { 
        IsAuthenticated = HttpContext?.User?.Identity?.IsAuthenticated ?? false,
        UserName = HttpContext?.User?.Identity?.Name ?? "Not logged in"
    };
}