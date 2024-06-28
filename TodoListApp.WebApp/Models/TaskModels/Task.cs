using Microsoft.Build.Framework;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models.TaskModels;

public class Task
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Title { get; set; }

    public ToDoTaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int TodoListId { get; set; }

    public string UserId { get; set; } = string.Empty;
}
