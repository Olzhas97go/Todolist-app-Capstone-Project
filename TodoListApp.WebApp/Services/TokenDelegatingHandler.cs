using System.Net.Http.Headers;

namespace TodoListApp.WebApp.Services;

public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public TokenDelegatingHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var jwtCookieName = this._configuration["JwtCookieName"] ?? "jwtToken";

        if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(jwtCookieName, out var token) == true)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
