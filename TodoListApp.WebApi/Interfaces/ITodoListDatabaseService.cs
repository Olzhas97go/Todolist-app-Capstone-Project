using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;

public interface ITodoListDatabaseService
{
        Task<List<TodoListModel>> GetAllTodoListsAsync();

        Task<TodoListDto> CreateTodoListAsync(TodoListDto newTodoListDto);

        Task<bool> DeleteTodoListAsync(int id);

        Task<TodoListEntity> UpdateTodoListAsync(int id, TodoListEntity todoList);

        Task<TodoListEntity> GetTodoListByIdAsync(int id);

        Task<bool> TodoListExists(int id);
}
