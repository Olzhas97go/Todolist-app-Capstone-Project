namespace TodoListApp.WebApi.Entities;

public class TodoListDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<int> TaskIds { get; set; }
}
