using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models.TaskModels;
public class TodoTaskViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public ToDoTaskStatus Status { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int TodoListId { get; set; }
    public bool IsOverdue { get; set; }
    public string UserId { get; set; }
}
