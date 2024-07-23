using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Data.TodoListApp;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class DatabaseRoleAssignmentService : IRoleAssignmentService
{
    private readonly WebAppContext _context;

    public DatabaseRoleAssignmentService(WebAppContext context)
    {
        _context = context;
    }

    public async Task<string> DetermineRoleAsync(string email)
    {
        // Ensure you're working with an IQueryable<IdentityUserRole<string>>
        IQueryable<IdentityUserRole<string>> userRoles = _context.UserRoles;

        var roleName = await userRoles // Use IQueryable<IdentityUserRole<string>>
            .Where(ur => ur.UserId == email) // Use IQueryable.Where
            .Select(ur => ur.RoleId)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(roleName))
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleName);
            return role?.Name; // Return the role name or null if not found
        }
        else
        {
            return null;
        }
    }
}

