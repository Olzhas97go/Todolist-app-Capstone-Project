namespace TodoListApp.WebApi.Models;

public class TodoListModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
