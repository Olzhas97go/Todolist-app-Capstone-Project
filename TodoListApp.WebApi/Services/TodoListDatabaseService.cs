namespace TodoListApp.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext _context;
    private readonly ILogger<TodoListDatabaseService> _logger;
    private readonly IMapper _mapper;

    public TodoListDatabaseService(TodoListDbContext context, ILogger<TodoListDatabaseService> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> TodoListExists(int id)
    {
        return await _context.TodoLists.AnyAsync(e => e.Id == id);
    }

    public async Task<List<TodoListModel>> GetAllTodoListsAsync()
    {
        try
        {
            var entities = await _context.TodoLists.ToListAsync(); // Query the DbSet
            return _mapper.Map<List<TodoListModel>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving todo lists.");
            throw new Exception("An error occurred while retrieving todo lists.", ex);
        }
    }

    public async Task<TodoListDto> CreateTodoListAsync(TodoListDto newTodoListDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newTodoListDto.Name))
            {
                throw new ArgumentException("Todo list name cannot be empty.");
            }

            var todoListEntity = _mapper.Map<TodoListEntity>(newTodoListDto);

            // Убедитесь, что идентификатор не установлен вручную
            todoListEntity.Id = 0;

            _context.TodoLists.Add(todoListEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TodoListDto>(todoListEntity);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid input while creating a todo list.");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while creating a todo list.");
            throw new Exception("An error occurred while saving the data.", ex);
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

    public async Task<TodoListEntity> UpdateTodoListAsync(int id, TodoListEntity todoList)
    {
        var existingTodoList = await _context.TodoLists.FindAsync(id);
        if (existingTodoList == null)
        {
            return null;
        }

        existingTodoList.Name = todoList.Name; // Update reference
        existingTodoList.Description = todoList.Description; // Update reference


        // ... (update other properties as needed)

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency exception here if needed
            throw;
        }

        return existingTodoList;
    }


    public List<TodoListModel> GetTasksForUser(string userId, ToDoTaskStatus? status = null, string sortBy = "Name", string sortOrder = "asc")
    {
        var query = _context.Tasks.Where(t => t.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status);
        }

        // Apply sorting
        switch (sortBy.ToLower())
        {
            case "name":
                query = sortOrder == "desc" ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title);
                break;
            case "duedate":
                query = sortOrder == "desc" ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate);
                break;
            default:
                // Default sorting (by Name ascending) or throw an exception for invalid input
                query = query.OrderBy(t => t.Title);
                break;
        }

        return query
            .Select(t => new TodoListModel { Id = t.Id, Name = t.Title, Description = t.Description, Status = t.Status })
            .ToList();
    }

    public async Task<TodoListEntity> GetTodoListByIdAsync(int id)
    {
        var todoListEntity = await _context.TodoLists
            .Include(tl => tl.Tasks) // Eager loading
            .FirstOrDefaultAsync(tl => tl.Id == id);

        if (todoListEntity == null)
        {
            return null;  // Return null if no to-do list is found
        }
        return todoListEntity;
    }
}
