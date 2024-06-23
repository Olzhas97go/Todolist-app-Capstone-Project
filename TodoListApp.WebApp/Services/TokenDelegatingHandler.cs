using System.Net.Http.Headers;

namespace TodoListApp.WebApp.Services;

public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public TokenDelegatingHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Get Cookie Name with Default
        var jwtCookieName = _configuration["JwtCookieName"] ?? "jwtToken"; // Provide a default

        // 2. Check if Cookie Exists
        if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(jwtCookieName, out var token) == true)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Continue Request
        return await base.SendAsync(request, cancellationToken);
    }

}
