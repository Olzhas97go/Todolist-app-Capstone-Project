namespace TodoListApp.WebApi.Services;

using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.DataContext;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext _context;
    private readonly ILogger<TodoListDatabaseService> _logger;

    public TodoListDatabaseService(TodoListDbContext context, ILogger<TodoListDatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TodoListModel>> GetAllTodoListsAsync()
    {
        return await _context.TodoLists
            .Select(entity => new TodoListModel {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description, // Added Description mapping
            })
            .ToListAsync();
    }

    public async Task<TodoList> CreateTodoListAsync(TodoList newTodoList)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newTodoList.Name))
            {
                throw new ArgumentException("Todo list name cannot be empty.");
            }

            var todoListEntity = new TodoListEntity { Name = newTodoList.Name, Description = newTodoList.Description };
            _context.TodoLists.Add(todoListEntity);
            await _context.SaveChangesAsync();

            return new TodoList { Id = todoListEntity.Id, Name = todoListEntity.Name, Description = todoListEntity.Description };
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid input while creating a todo list.");
            throw; // Re-throw the exception for the controller to handle
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while creating a todo list.");
            throw new Exception("An error occurred while saving the data.", ex); // Wrap the exception for more context
        }
    }

    public async Task<bool> DeleteTodoListAsync(int id)
    {
        try
        {
            var todoList = await _context.TodoLists.FindAsync(id);
            if (todoList == null)
            {
                return false; // Not found
            }

            _context.TodoLists.Remove(todoList);
            await _context.SaveChangesAsync();
            return true; // Successfully deleted
        }
        catch (DbUpdateException ex)
        {
            _logger?.LogError(ex, "An error occurred while deleting a todo list with ID {Id}", id);
            throw; // Let the controller handle the exception
        }
    }

    public async Task<TodoList?> UpdateTodoListAsync(int id, TodoList updatedTodoList)
    {
        try
        {
            var existingTodoList = await _context.TodoLists.FindAsync(id);
            if (existingTodoList == null)
            {
                return null; // Not found
            }

            existingTodoList.Name = updatedTodoList.Name;
            existingTodoList.Description = updatedTodoList.Description;

            await _context.SaveChangesAsync();

            return new TodoList
            {
                Id = existingTodoList.Id,
                Name = existingTodoList.Name,
                Description = existingTodoList.Description
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger?.LogError(ex, "Concurrency error updating TodoList with ID {Id}", id);
            throw; // Let the controller handle the concurrency conflict
        }
        catch (DbUpdateException ex)
        {
            _logger?.LogError(ex, "Error updating TodoList with ID {Id}", id);
            throw; // Let the controller handle the database error
        }
    }
}
