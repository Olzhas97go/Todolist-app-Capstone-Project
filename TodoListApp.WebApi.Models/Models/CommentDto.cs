namespace TodoListApp.WebApi.Models.Models;
public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int TaskId { get; set; }
    public string User { get; set; }
}
