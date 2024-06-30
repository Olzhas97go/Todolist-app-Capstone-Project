using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoListApp.WebApp.Areas.Identity.Data;

namespace TodoListApp.WebApp.Areas.Identity.Pages.Account;

[AllowAnonymous] // Allow anonymous access for logout
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
    {
        this._signInManager = signInManager;
        this._logger = logger;
    }

    public void OnGet()
    {
        // Optional: You can add logic here if needed before logout
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await this._signInManager.SignOutAsync();
        this._logger.LogInformation("User logged out.");

        // Clear the JWT cookie
        Response.Cookies.Delete("jwtToken");

        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToPage();
        }
    }
}
