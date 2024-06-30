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

    [Get("/api/task/{todoListId}/tasks")] // Get tasks for a specific todo list
    Task<List<TodoTask>> GetTasksForTodoListAsync(int todoListId);

    [Post("/api/TodoList")]
    Task<TodoListDto> CreateTodoList([Body] TodoListDto todoList);

    // ITodoListApi interface
    [Put("/api/todolist/{id}")]
    Task UpdateTodoList(int id, [Body] TodoListDto todoList);

    [Delete("/api/todolist/{id}")]  // Make sure you have the [Delete] attribute and a string literal for the path
    Task DeleteTodoList(int id);

    [Get("/api/task/{taskId}")]
    Task<TaskEntity> GetTaskByIdAsync(int taskId);
    [Post("/api/task/{todoListId}/tasks")] // Update the route
    Task<TodoTaskDto> AddTask(int todoListId, [Body] TodoTaskDto taskDto);
    [Delete("/api/task/{taskId}")]
    Task DeleteTask(int taskId);
    [Put("/api/task/{taskId}")]  // PUT method for updating
    Task<TodoTaskDto> UpdateTask(int taskId, [Body] TodoTaskDto updatedTask);
// In ITodoListApi.cs
    [Get("/api/task/GetMyTasks")]
    Task<List<TodoTask>> GetMyTasks([Query] string userId, [Query] string status = null, [Query] string sortBy = "Name", [Query] string sortOrder = "asc", [Query] string searchString = null);

    // In your ITodoListApi interface
    // In your ITodoListApi interface
    // In your ITodoListApi interface
    [Put("/api/tasks/{taskId}/status")] // Corrected endpoint
    Task<TodoTaskDto> UpdateTaskStatusAsync(int taskId, [Body] UpdateTaskStatusRequest request);
    [Get("/api/task/search")] // Use the correct path based on your API route
    Task<IEnumerable<TodoTaskDto>> Search([Query] string title);
    [Get("/api/tags/task/{taskId}")]
    Task<IEnumerable<TagDto>> GetTagsForTaskAsync(int taskId);
    // In your ITodoListApi interface
    [Get("/api/tags/tasks/{tagText}")]
    Task<ApiResponse<IEnumerable<TodoTaskDto>>> GetTasksByTagAsync(string tagText);

    [Post("/api/tags/create")]
    Task<ApiResponse<TagDto>> CreateTagAsync(TagDto tagDto);

    // [Delete("api/tags/delete/{id}")]
    // Task<HttpResponseMessage> DeleteTagAsync(int id);
}
