using Models.Values;

namespace Tests.Services;


public class TestAuthenticationService : IAuthenticationService
{
    public AppUser GetAppUser() => new(
        IsAuthenticated: IsAuthenticated(),
        UserName: GetUserName()
    );

    public string GetUserName() => "TestUser";

    public bool IsAuthenticated() => true;
}