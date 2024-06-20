using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Models;

public class TodoListWebApiModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters.")]
    public string Name { get; set; } = string.Empty;
    [StringLength(250, ErrorMessage = "Description cannot be more than 250 characters.")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "The Tasks field is required.")]
    public List<TodoTaskDto> Tasks { get; set; } = new();
}
