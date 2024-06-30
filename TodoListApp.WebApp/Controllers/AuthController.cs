// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;
// using TodoListApp.WebApp.Areas.Identity.Data;
// using TodoListApp.WebApp.Data.TodoListApp;
//
// namespace TodoListApp.WebApp.Controllers
// {
//     [ApiController]
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("api/[controller]")]
//     public class AuthController : Controller
//     {
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly SignInManager<ApplicationUser> _signInManager;
//         private readonly IConfiguration _config;
//
//         public AuthController(
//             UserManager<ApplicationUser> userManager,
//             SignInManager<ApplicationUser> signInManager,
//             IConfiguration config)
//         {
//             _userManager = userManager;
//             _signInManager = signInManager;
//             _config = config;
//         }
//
//         // GET endpoint to log in and get a token
//         [AllowAnonymous]
//         [HttpPost("getToken")]
//         public async Task<IActionResult> GetToken([FromBody] LoginModel loginModel)
//         {
//             var user = await this._userManager.FindByEmailAsync(loginModel.EmailAddress);
//
//             if (user != null)
//             {
//                 var result = await this._signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
//
//                 if (result.Succeeded)
//                 {
//                     var tokenHandler = new JwtSecurityTokenHandler();
//                     var key = Encoding.ASCII.GetBytes(this._config.GetSection("Keys")["TokenSigningKey"]);
//                     var tokenDescriptor = new SecurityTokenDescriptor
//                     {
//                         Subject = new ClaimsIdentity(new Claim[]
//                         {
//                             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                         }),
//                         Expires = DateTime.UtcNow.AddDays(30),
//                         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
//                     };
//
//                     var token = tokenHandler.CreateToken(tokenDescriptor);
//                     var tokenString = tokenHandler.WriteToken(token);
//
//                     this.Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
//                     {
//                         HttpOnly = true,
//                         Secure = true,
//                         SameSite = SameSiteMode.Strict,
//                         Expires = DateTime.UtcNow.AddDays(1),
//                     });
//                     return this.Ok(new { Token = tokenString });
//                 }
//
//                 return this.Unauthorized("Invalid credentials.");
//             }
//
//             return this.NotFound("User not found.");
//         }
//
//         [HttpGet("getResources")]
//         public IActionResult GetResources()
//         {
//             return this.Ok(new { Data = "THIS IS THE DATA THAT IS PROTECTED BY AUTHORIZATION" });
//         }
//     }
// }
