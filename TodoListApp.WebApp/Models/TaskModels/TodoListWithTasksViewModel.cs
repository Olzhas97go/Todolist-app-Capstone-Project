using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Models.TaskModels;

public class TodoListWithTasksViewModel
{
    public string UserId { get; set; }

    public TodoListDetailsDto TodoList { get; set; }

    public List<TodoListModel> Tasks { get; set; } = new List<TodoListModel>();

    public ToDoTaskStatus? StatusFilter { get; set; }

    public string SortBy { get; set; } = "Name"; // Default sort by name

    public string SortOrder { get; set; } = "asc";

    public string SearchString { get; set; } = "";
}
