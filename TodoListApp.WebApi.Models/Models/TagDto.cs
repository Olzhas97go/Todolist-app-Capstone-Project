    namespace TodoListApp.WebApi.Models.Models;

    public class TagDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int TaskId { get; set; }

        public IEnumerable<TodoTaskDto> Tasks { get; set; } = new List<TodoTaskDto>();
    }
