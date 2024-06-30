using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Entities;

public class CommentEntity
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int TaskId { get; set; }

    public TaskEntity? Task { get; set; }

    public string User { get; set; } = string.Empty;
}
