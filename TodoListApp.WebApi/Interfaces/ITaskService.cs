namespace TodoListApp.WebApi.Interfaces;

using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApi.Models;

public interface ITaskService
{
    Task<List<TodoTask>> GetTasksForTodoListAsync(int todoListId);  // US05

    Task<TodoTask> GetTaskByIdAsync(int taskId);                   // US06

    Task<TodoTask> AddTaskAsync(TaskEntity taskEntity); // US07

    Task<bool> DeleteTaskAsync(int taskId);                         // US08

    Task<TodoTask> UpdateTaskAsync(int taskId, TodoTask updatedTodoTask);

    Task<TodoTask> UpdateTaskStatusAsync(int todoListId, int taskId, ToDoTaskStatus newStatus);
}
