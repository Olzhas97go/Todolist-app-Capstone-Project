using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Entities;
public class TagEntity
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Text { get; set; } = string.Empty;

    public int TaskId { get; set; }
    public TaskEntity? Task { get; set; }
}
