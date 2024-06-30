using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; } = null!;

    public UserRoles Role { get; set; }
}

