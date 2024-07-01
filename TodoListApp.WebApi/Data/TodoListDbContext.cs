using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Entities;

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

        modelBuilder.Entity<TaskEntity>()
            .HasOne<TodoListEntity>(t => t.TodoList)
            .WithMany(tl => tl.Tasks)
            .HasForeignKey(t => t.TodoListId);

        modelBuilder.Entity<TagEntity>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Text).HasMaxLength(20);

            entity.HasOne(t => t.Task)
                .WithMany(t => t.Tags)
                .HasForeignKey(t => t.TaskId);
        });

        modelBuilder.Entity<CommentEntity>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Text).IsRequired();
            entity.Property(c => c.CreatedDate).IsRequired();

            entity.HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);
        });

    }
}
