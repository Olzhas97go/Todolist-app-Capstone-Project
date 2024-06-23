using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Models;

public class TodoTask
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int TodoListId { get; set; }

    public bool IsOverdue { get; set; } // For US10
    public ToDoTaskStatus Status { get; set; }
}
