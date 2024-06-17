    namespace TodoListApp.WebApi.Interfaces;

    using TodoListApp.WebApi.Models;

    public interface ITodoListDatabaseService
    {
        Task<List<TodoListModel>> GetAllTodoListsAsync();

        Task<TodoList> CreateTodoListAsync(TodoList newTodoList);

        Task<bool> DeleteTodoListAsync(int id);

        Task<TodoList?> UpdateTodoListAsync(int id, TodoList updatedTodoList);

        List<TodoListModel> GetTasksForUser(string userId, ToDoTaskStatus? status = null, string sortBy = "Name", string sortOrder = "asc");
    }
