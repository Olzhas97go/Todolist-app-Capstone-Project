using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Models;

public class TodoListWithTasksViewModel
{
    public TodoListDetailsDto TodoList { get; set; }
    public List<TodoTask> Tasks { get; set; }
}
