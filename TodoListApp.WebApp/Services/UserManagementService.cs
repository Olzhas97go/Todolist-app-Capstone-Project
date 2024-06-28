using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApp.Areas.Identity.Data;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class UserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Implement IUserManager methods, e.g.:
    public async Task<string> GetUserId(ClaimsPrincipal user)
    {
        var applicationUser = await _userManager.GetUserAsync(user); // Get ApplicationUser from ClaimsPrincipal
        if (applicationUser != null)
        {
            return await _userManager.GetUserIdAsync(applicationUser);  // Now pass ApplicationUser
        }
        else
        {
            // Handle the case where the user is not found (e.g., log an error, throw an exception)
            throw new InvalidOperationException("User not found.");
        }
    }
}
