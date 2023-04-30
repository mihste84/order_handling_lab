namespace AppLogic.Common.Interfaces;


public interface IAuthenticationService
{
    AppUser GetAppUser();
    bool IsAuthenticated();
    string GetUserName();
}