using AppLogic.Common.Interfaces;
using Models.Values;

namespace Services.Authentication;


public class TestAuthenticationService : IAuthenticationService
{
    public AppUser GetAppUser() => new(
        IsAuthenticated: IsAuthenticated(),
        UserName: GetUserName()
    );

    public string GetUserName() => "TestUser";

    public bool IsAuthenticated() => true;
}