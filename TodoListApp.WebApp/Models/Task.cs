using Microsoft.Build.Framework;

namespace TodoListApp.WebApp.Models;

public class Task
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public bool Completed { get; set; } = false;

}
