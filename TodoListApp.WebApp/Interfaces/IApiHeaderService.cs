namespace TodoListApp.WebApp.Interfaces;

public interface IApiHeaderService
{
    Task AddJwtAuthorizationHeader(HttpContext context);
}
