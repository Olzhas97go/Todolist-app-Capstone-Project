using System.Security.Claims;

namespace TodoListApp.WebApp.Interfaces;

public interface IUserManager
{
    Task<string> GetUserId(ClaimsPrincipal user);
}
