using Common.Interfaces;

namespace App.Authentication;

public class TestAuthenticationService : IAuthenticationService
{
    public AppUser GetAppUser() => new(
        IsAuthenticated: IsAuthenticated(),
        UserName: GetUserName()
    );

    public bool IsAuthenticated() => true;

    public string GetUserName() => "test_user@mail.com";
}

public class AllowAnonymous : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}