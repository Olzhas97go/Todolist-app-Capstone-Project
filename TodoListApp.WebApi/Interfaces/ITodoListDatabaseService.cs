    namespace TodoListApp.WebApi.Interfaces;

    using TodoListApp.WebApi.Models;

    public interface ITodoListDatabaseService
    {
        Task<List<TodoListModel>> GetAllTodoListsAsync();

        Task<TodoListDto> CreateTodoListAsync(TodoListDto newTodoListDto);

        Task<bool> DeleteTodoListAsync(int id);

        Task<TodoListEntity> UpdateTodoListAsync(int id, TodoListEntity todoList);

        List<TodoListModel> GetTasksForUser(string userId, ToDoTaskStatus? status = null, string sortBy = "Name", string sortOrder = "asc");

        Task<TodoListEntity> GetTodoListByIdAsync(int id);

        Task<bool> TodoListExists(int id);
    }
