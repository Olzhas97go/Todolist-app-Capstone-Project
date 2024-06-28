namespace TodoListApp.WebApi.Models;
public class UpdateTaskStatusRequest
{
    public ToDoTaskStatus NewStatus { get; set; }
    public int TodoListId { get; set; }
}
