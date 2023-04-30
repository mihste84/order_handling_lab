using AppLogic.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Models.Values;

namespace Services.Authentication;


public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AppUser GetAppUser() => new(
        IsAuthenticated: IsAuthenticated(),
        UserName: GetUserName()
    );

    public bool IsAuthenticated() => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string GetUserName() => _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Not logged in";
}