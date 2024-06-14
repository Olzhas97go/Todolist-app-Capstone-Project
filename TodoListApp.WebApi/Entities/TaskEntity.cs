namespace TodoListApp.WebApi.Models.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;

public class TaskEntity
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = "No description";
    public ToDoTaskStatus Status { get; set; } = ToDoTaskStatus.NotStarted;

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    // Relationship with TodoListEntity
    [ForeignKey("TodoList")]
    public int TodoListId { get; set; }

    [ForeignKey("TodoListId")]
    public TodoListEntity TodoList { get; set; } = null!;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
}
