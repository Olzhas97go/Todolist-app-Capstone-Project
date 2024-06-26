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
    [Get("/api/Task/GetMyTasks")]
    Task<List<TodoTask>> GetMyTasks([Query] string userId, [Query] ToDoTaskStatus? status = null, [Query] string sortBy = "Name", [Query] string sortOrder = "asc");

}
