namespace TodoListApp.WebApi.Models.Tasks;


using System.ComponentModel.DataAnnotations;

public class TaskModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int TodoListId { get; set; }

    public bool IsOverdue { get; set; } // For US10
}
