using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Entities;


public class TodoListEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public string UserId { get; set; } = string.Empty;

    public List<TaskEntity> Tasks { get; set; } = new();
}
