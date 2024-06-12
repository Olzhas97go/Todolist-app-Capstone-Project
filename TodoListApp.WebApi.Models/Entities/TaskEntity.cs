namespace TodoListApp.WebApi.Models.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class TaskEntity
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = "No description";
    public Status Status { get; set; } = Status.NotStarted;

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
    // Add a User property if you're implementing authentication
    //public UserEntity? AssignedTo { get; set; }
}
