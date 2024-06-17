using Microsoft.AspNetCore.Mvc;
using Refit;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITodoListApi
{
    [Get("/api/TodoList")]
    Task<List<TodoListDto>> GetAllTodoLists();

    [Get("/api/TodoList/{id}")]
    Task<TodoListDto> GetTodoListById(int id);

    [Post("/api/TodoList")]
    Task<TodoListDto> CreateTodoList([Body] TodoListDto todoList);

    // ITodoListApi interface
    [Put("/api/todolist/{id}")]
    Task UpdateTodoList(int id, [Body] TodoListDto todoList);


    [Delete("/api/todolist/{id}")]  // Make sure you have the [Delete] attribute and a string literal for the path
    Task DeleteTodoList(int id);
}
