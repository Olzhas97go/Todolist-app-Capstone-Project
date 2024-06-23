using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class ApiHeaderService : IApiHeaderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public ApiHeaderService(IJwtTokenGenerator jwtTokenGenerator, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task AddJwtAuthorizationHeader(HttpContext context)
    {
        var tokenSigningKey = _configuration["Jwt:TokenSigningKey"]; // Default to "jwtToken"
        var token = context.Request.Cookies[tokenSigningKey];

        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetTokenValidationParameters(); // Define your validation parameters

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var jwtProvider = _serviceProvider.GetRequiredService<IJwtTokenGenerator>(); // Resolve IJwtTokenGenerator
                var newToken = await jwtProvider.GenerateToken(principal);
                context.Request.Headers["Authorization"] = $"Bearer {newToken}";
            }
        }
        catch (SecurityTokenExpiredException)
        {
            // Token expired: return 401 Unauthorized with specific error message
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token is expired\"";
            return;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            // Invalid signature: return 401 Unauthorized with specific error message
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token has an invalid signature\"";
            return;
        }
        catch (SecurityTokenException ex) // More specific than just Exception
        {
            // Other token validation errors (e.g., invalid issuer or audience)
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token is invalid\"";
            return;
        }
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        var tokenSigningKey = _configuration["Jwt:TokenSigningKey"];
        var validIssuer = _configuration["Jwt:ValidIssuer"];
        var validAudience = _configuration["Jwt:ValidAudience"];

        if (string.IsNullOrEmpty(tokenSigningKey) || string.IsNullOrEmpty(validIssuer) || string.IsNullOrEmpty(validAudience))
        {
            throw new InvalidOperationException("JWT configuration values (TokenSigningKey, ValidIssuer, or ValidAudience) are missing or invalid.");
        }
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtOptions:ValidIssuer"],
            ValidAudience = _configuration["JwtOptions:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Keys:TokenSigningKey"]))
        };
    }
}

