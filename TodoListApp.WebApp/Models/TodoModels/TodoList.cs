using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Models;

public class TodoList
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<TodoTaskDto> Tasks { get; set; } = new List<TodoTaskDto>();

}
