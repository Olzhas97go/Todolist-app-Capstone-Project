using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Areas.Identity.Data;

public class ApplicationUser : IdentityUser, IUserManager
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; } = null!;

    public async Task<string> GetUserId(ClaimsPrincipal user)
    {
        return await Task.FromResult(this.Id); // Or however you retrieve the user ID
    }
}

