namespace TodoListApp.WebApi.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models.Identity;

public class UserDbContext : IdentityDbContext<User>
{
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)  // Removed cast
        {
        }
    }
