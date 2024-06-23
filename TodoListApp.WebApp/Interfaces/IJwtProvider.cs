using System.Security.Claims;

namespace TodoListApp.WebApp.Interfaces;

public interface IJwtProvider
{
    Task<string> GenerateToken(ClaimsPrincipal user);
}


