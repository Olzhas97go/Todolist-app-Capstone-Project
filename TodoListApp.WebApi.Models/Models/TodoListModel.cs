namespace TodoListApp.WebApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TodoListModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [Range(1, 120)]
    public string? Description { get; set; }
}
