using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Areas.Identity.Data;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class JwtProvider : IJwtProvider, IJwtTokenGenerator
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public JwtProvider(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        this._configuration = _configuration;
    }

    public async Task<string> GenerateToken(ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst(ClaimTypes.Email);
        if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
        {
            throw new InvalidOperationException("User does not have an email.");
        }
        var email = emailClaim.Value; // Get the email value from the claim

        var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null || string.IsNullOrEmpty(nameIdentifierClaim.Value))
        {
            throw new InvalidOperationException("User does not have an Id.");
        }
        var userId = nameIdentifierClaim.Value; // Get the user ID value from the claim

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "Owner")
            // Add other relevant claims here if needed
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:TokenSigningKey"]);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var expires = DateTime.UtcNow.AddDays(7);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:ValidIssuer"],
            Audience = _configuration["Jwt:ValidAudience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
}
