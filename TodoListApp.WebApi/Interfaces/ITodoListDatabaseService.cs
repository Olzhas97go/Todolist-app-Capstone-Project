    namespace TodoListApp.WebApi.Interfaces;

    using TodoListApp.WebApi.Models;

    public interface ITodoListDatabaseService
    {
        Task<List<TodoListModel>> GetAllTodoListsAsync();

        Task<TodoList> CreateTodoListAsync(TodoList newTodoList);

        Task<bool> DeleteTodoListAsync(int id);

        Task<TodoList?> UpdateTodoListAsync(int id, TodoList updatedTodoList);
    }
