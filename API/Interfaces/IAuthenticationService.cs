namespace API.Interfaces;


public interface IAuthenticationService
{
    Task<string> GetUserNameAsync();
}