using System.Security.Claims;

namespace TodoListApp.WebApp.Interfaces;

public interface  IJwtTokenGenerator
{
    Task<string> GenerateToken(ClaimsPrincipal user);
}
