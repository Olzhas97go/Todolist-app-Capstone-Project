using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Areas.Identity.Data;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class JwtProvider : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    public JwtProvider(
        IConfiguration config)
    {
        this._config = config;
    }

    public async Task<string> GenerateToken(ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst(ClaimTypes.Email);
        if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
        {
            throw new InvalidOperationException("User does not have an email.");
        }

        var email = emailClaim.Value;

        var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null || string.IsNullOrEmpty(nameIdentifierClaim.Value))
        {
            throw new InvalidOperationException("User does not have an Id.");
        }

        var userId = nameIdentifierClaim.Value;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(this._config["Jwt:TokenSigningKey"]);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var expires = DateTime.UtcNow.AddDays(7);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = creds,
            Issuer = this._config["Jwt:ValidIssuer"],
            Audience = this._config["Jwt:ValidAudience"],
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
}
