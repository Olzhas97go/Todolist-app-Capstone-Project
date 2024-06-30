using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class ApiHeaderService : IApiHeaderService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public ApiHeaderService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
        this._configuration = configuration;
    }

    public async Task AddJwtAuthorizationHeader(HttpContext context)
    {
        var tokenSigningKey = this._configuration["Jwt:TokenSigningKey"];
        var token = context.Request.Cookies[tokenSigningKey];

        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = this.GetTokenValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var jwtProvider = this._serviceProvider.GetRequiredService<IJwtTokenGenerator>();
                var newToken = await jwtProvider.GenerateToken(principal);
                context.Request.Headers["Authorization"] = $"Bearer {newToken}";
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token is expired\"";
            return;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token has an invalid signature\"";
            return;
        }
        catch (SecurityTokenException ex)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\", error_description=\"The token is invalid\"";
            return;
        }
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        var tokenSigningKey = this._configuration["Jwt:TokenSigningKey"];
        var validIssuer = this._configuration["Jwt:ValidIssuer"];
        var validAudience = this._configuration["Jwt:ValidAudience"];

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
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Keys:TokenSigningKey"])),
        };
    }
}

