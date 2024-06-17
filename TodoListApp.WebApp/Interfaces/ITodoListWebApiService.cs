using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITodoListWebApiService
{
    Task<List<TodoListModel>> GetTasksForUserAsync();
    Task UpdateTaskStatusAsync(int taskId, ToDoTaskStatus newStatus);

    Task<List<TodoListDto>> GetTodoListsAsync();
    Task<TodoListDto> GetTodoListByIdAsync(int id);
    Task<TodoListDto> CreateTodoListAsync(TodoListDto todoListDto);
}
