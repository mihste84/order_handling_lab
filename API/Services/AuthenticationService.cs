namespace API.Services;


public class AuthenticationService : IAuthenticationService
{
    public Task<string> GetUserNameAsync() => Task.FromResult("Stefan");
}