using Models;

namespace SkischoolHub.Services;

public interface IUserSessionService
{
    User? CurrentUser { get; set; }
    void Clear();
}
