using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Models.Models;

public class TodoTaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; }
    public ToDoTaskStatus Status { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int TodoListId { get; set; }
    public bool IsOverdue { get; set; }
}
