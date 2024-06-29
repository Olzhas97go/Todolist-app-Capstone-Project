using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListApp.WebApi.Models.Models;

public class TodoTask
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int TodoListId { get; set; }

    public bool IsOverdue { get; set; } // For US10
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ToDoTaskStatus Status { get; set; }
    public string UserId { get; set; } = string.Empty;
}
