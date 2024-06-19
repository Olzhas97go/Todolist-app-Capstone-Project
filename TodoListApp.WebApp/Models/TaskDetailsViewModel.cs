using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Models;

public class TaskDetailsViewModel
{
    public TodoListDto TodoList { get; set; }
    public int TodoListId { get; set; }
    public TodoListApp.WebApp.Models.Task Task { get; set; }
    public List<TodoListApp.WebApp.Models.Task> TaskList { get; set; }
}
