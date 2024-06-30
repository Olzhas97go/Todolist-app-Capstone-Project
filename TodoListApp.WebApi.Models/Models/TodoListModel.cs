using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.WebApi.Models.Models;

public class TodoListModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [Range(1, 120)]
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ToDoTaskStatus Status { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; } = DateTime.UtcNow;
    public bool IsOverdue { get; set; }
    public bool IsCompleted { get; set; }
    public int TodoListId { get; set; }
}
