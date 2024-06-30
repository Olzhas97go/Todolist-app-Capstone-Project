using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Data;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TaskEntity> Tasks { get; set; }

    public DbSet<TagEntity> Tags { get; set; }

    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoListEntity>()
            .HasMany<TaskEntity>(tl => tl.Tasks)
            .WithOne(t => t.TodoList)
            .HasForeignKey(t => t.TodoListId);

        modelBuilder.Entity<TaskEntity>() // Add this if you haven't already
            .HasOne<TodoListEntity>(t => t.TodoList)
            .WithMany(tl => tl.Tasks)
            .HasForeignKey(t => t.TodoListId);

        modelBuilder.Entity<TagEntity>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Text).HasMaxLength(20);

            // Define the relationship between TagEntity and TaskEntity
            entity.HasOne(t => t.Task)
                .WithMany(t => t.Tags) // Assuming you have a 'Tags' collection in TaskEntity
                .HasForeignKey(t => t.TaskId);
        });

        modelBuilder.Entity<CommentEntity>(entity =>
        {
            entity.HasKey(c => c.Id); // Set the primary key
            entity.Property(c => c.Text).IsRequired(); // Text is required
            entity.Property(c => c.CreatedDate).IsRequired();

            // Define the relationship with TaskEntity (Many-to-One)
            entity.HasOne(c => c.Task)
                .WithMany(t => t.Comments) // TaskEntity should have a Comments collection
                .HasForeignKey(c => c.TaskId);
        });

    }
}
