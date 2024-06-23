using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class JwtTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider

    public JwtTokenMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Create a new scope within each request.
        using (var scope = _serviceProvider.CreateScope())
        {
            var apiHeaderService = scope.ServiceProvider.GetRequiredService<IApiHeaderService>();
            // Use apiHeaderService here
            await apiHeaderService.AddJwtAuthorizationHeader(context);
        }

        await _next(context);
    }
}

