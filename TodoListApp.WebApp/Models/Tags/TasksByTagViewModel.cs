namespace TodoListApp.WebApp.Models.TaskModels;
public class TasksByTagViewModel
{
    public string TagText { get; set; }

    public IEnumerable<TodoListWithTasksViewModel> Tasks { get; set; }
}
