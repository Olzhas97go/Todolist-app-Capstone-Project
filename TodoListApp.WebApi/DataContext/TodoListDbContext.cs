namespace TodoListApp.WebApi.DataContext;

using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }
}
