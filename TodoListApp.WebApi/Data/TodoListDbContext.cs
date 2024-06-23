﻿using Microsoft.AspNetCore.Identity;
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
    }
}
