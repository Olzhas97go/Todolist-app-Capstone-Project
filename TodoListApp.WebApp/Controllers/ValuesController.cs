using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Areas.Identity.Data;
using TodoListApp.WebApp.Data.TodoListApp;

namespace TodoListApp.WebApp.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly WebAppContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(
            WebAppContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            ILogger<ValuesController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // GET endpoint to log in and get a token
        [AllowAnonymous]
        [HttpPost("getToken")]
        public async Task<IActionResult> GetToken([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.EmailAddress);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

                if (result.Succeeded)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_config.GetSection("Keys")["TokenSigningKey"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Use UserId claim
                        }),
                        Expires = DateTime.UtcNow.AddDays(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // Set HttpOnly Cookie with SameSite Mode
                    Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(30) // Set expiration
                    });
                    return Ok(new { Token = tokenString }); // You can optionally return the token string
                }

                return Unauthorized("Invalid credentials.");
            }

            return NotFound("User not found.");
        }

        [HttpGet("getResources")]
        public IActionResult GetResources()
        {
            return Ok(new { Data = "THIS IS THE DATA THAT IS PROTECTED BY AUTHORIZATION" });
        }
    }
}
