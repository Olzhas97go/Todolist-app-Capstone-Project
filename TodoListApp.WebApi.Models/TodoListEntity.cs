namespace TodoListApp.WebApi.Models;

using System.ComponentModel.DataAnnotations;

public class TodoListEntity
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
