using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class JwtTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public JwtTokenMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        this._next = next;
        this._serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = this._serviceProvider.CreateScope())
        {
            var apiHeaderService = scope.ServiceProvider.GetRequiredService<IApiHeaderService>();
            await apiHeaderService.AddJwtAuthorizationHeader(context);
        }

        await this._next(context);
    }
}

