using Microsoft.AspNetCore.Mvc;
using Refit;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITodoListApi
{
    [Get("/api/TodoList")]
    Task<List<TodoListDto>> GetAllTodoLists();

    [Get("/api/TodoList/{id}")]
    Task<TodoListDetailsDto> GetTodoListById(int id);

    [Get("/api/task/{todoListId}/tasks")]
    Task<List<TodoTask>> GetTasksForTodoListAsync(int todoListId);

    [Post("/api/TodoList")]
    Task<TodoListDto> CreateTodoList([Body] TodoListDto todoList);

    [Put("/api/todolist/{id}")]
    Task UpdateTodoList(int id, [Body] TodoListDto todoList);

    [Delete("/api/todolist/{id}")]
    Task DeleteTodoList(int id);

    [Get("/api/task/{taskId}")]
    Task<TaskEntity> GetTaskByIdAsync(int taskId);

    [Post("/api/task/{todoListId}/tasks")]
    Task<TodoTaskDto> AddTask(int todoListId, [Body] TodoTaskDto taskDto);

    [Delete("/api/task/{taskId}")]
    Task DeleteTask(int taskId);

    [Put("/api/task/{taskId}")]
    Task<TodoTaskDto> UpdateTask(int taskId, [Body] TodoTaskDto updatedTask);

    [Get("/api/task/GetMyTasks")]
    Task<List<TodoTask>> GetMyTasks([Query] string userId, [Query] string status = null, [Query] string sortBy = "Name", [Query] string sortOrder = "asc", [Query] string searchString = null);

    [Put("/api/tasks/{taskId}/status")]
    Task<TodoTaskDto> UpdateTaskStatusAsync(int taskId, [Body] UpdateTaskStatusRequest request);

    [Get("/api/task/search")]
    Task<IEnumerable<TodoTaskDto>> Search([Query] string title);

    [Get("/api/tags/task/{taskId}")]
    Task<IEnumerable<TagDto>> GetTagsForTaskAsync(int taskId);

    [Get("/api/tags/tasks/{tagText}")]
    Task<ApiResponse<IEnumerable<TodoTaskDto>>> GetTasksByTagAsync(string tagText);

    [Post("/api/tags/create")]
    Task<ApiResponse<TagDto>> CreateTagAsync(TagDto tagDto);

    // [Delete("api/tags/delete/{id}")]
    // Task<HttpResponseMessage> DeleteTagAsync(int id);
}
