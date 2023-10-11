namespace home_control;

public record LoginUserRequest(string UserName, string Password);

public record LoginUserResponse(bool Success, string UserName, Guid? id);

public interface IUserService
{
    LoginUserResponse TryLogin(LoginUserRequest request);

}
public class UserService : IUserService
{
    public static Guid UserGuid = Guid.NewGuid();
    public UserService()
    {
    }

    public LoginUserResponse TryLogin(LoginUserRequest request)
    {
        if (request.UserName == "mciec" && request.Password == "a")
        {
            return new LoginUserResponse(true, request.UserName, UserGuid);
        }
        return new LoginUserResponse(false, request.UserName, (Guid?)null);
    }

}
