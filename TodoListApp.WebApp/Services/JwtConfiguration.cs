using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoListApp.WebApp.Services;

public class JwtConfiguration
{
    private readonly IConfiguration _configuration;

    public JwtConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        var tokenSigningKey = _configuration["Jwt:TokenSigningKey"];
        if (string.IsNullOrEmpty(tokenSigningKey))
        {
            throw new InvalidOperationException("JWT TokenSigningKey is not found in the configuration.");
        }

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:ValidIssuer"],     // Corrected path
            ValidAudience = _configuration["Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSigningKey))
        };
    }
}
