using Microsoft.Build.Framework;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models.TaskModels;

public class Task
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }
    [Required]
    public string Title { get; set; } // Add this property

    public ToDoTaskStatus Status { get; set; } // Add this property


    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public bool Completed { get; set; } = false;

    public bool IsCompleted { get; set; } = false;
    public int TodoListId { get; set; }
}
