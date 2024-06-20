namespace TodoListApp.WebApi.Models;

using TodoListApp.WebApi.Models.Models;

public class TodoListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ToDoTaskStatus Status { get; set; }

    public List<TodoTaskDto> Tasks { get; set; } = new();
}
