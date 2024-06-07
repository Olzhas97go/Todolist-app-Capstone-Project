namespace TodoListApp.WebApi.Data;

using Microsoft.AspNetCore.Identity;

public class UserSeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Owner", "Editor", "Viewer", "Authorized", "Unauthorized" };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new { Username = "owner1", Email = "owner1@example.com", Role = "Owner" },
            new { Username = "editor1", Email = "editor1@example.com", Role = "Editor" },
            new { Username = "viewer1", Email = "viewer1@example.com", Role = "Viewer" },
            new { Username = "authorized", Email = "authorized@example.com", Role = "Authorized" },
            new { Username = "unauthorized", Email = "unauthorized@example.com", Role = "Unauthorized" },
        };

        foreach (var user in users)
        {
            var identityUser = new IdentityUser { UserName = user.Username, Email = user.Email };
            var result = await _userManager.CreateAsync(identityUser, "StrongPassword123!"); // Use a strong password
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, user.Role);
            }
        }
    }
}
