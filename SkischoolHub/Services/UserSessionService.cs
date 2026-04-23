using Models;

namespace SkischoolHub.Services;

public class UserSessionService : IUserSessionService
{
    public User? CurrentUser { get; set; }

    public void Clear()
    {
        CurrentUser = null;
    }
}
