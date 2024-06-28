using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Services;



public class TaskService : ITaskService
{
    private readonly TodoListDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    public TaskService(TodoListDbContext context, IMapper mapper, ILogger<TaskService> logger)
    {
    _context = context;
    _mapper = mapper;
    _logger = logger;
    }


    public async Task<List<TodoTask>> GetTasksForTodoListAsync(int todoListId)
    {
        try
        {
            var taskEntities = await _context.Tasks
                .Where(t => t.TodoListId == todoListId)
                .ToListAsync();

            var taskModels = _mapper.Map<List<TodoTask>>(taskEntities);

            // Determine if a task is overdue and set IsOverdue property
            foreach (var task in taskModels)
            {
                task.IsOverdue = task.DueDate.HasValue && task.DueDate.Value < DateTime.Now;
            }

            return taskModels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving tasks for todo list with ID {todoListId}.", todoListId);
            throw;
        }
    }

    public async Task<TodoTask> GetTaskByIdAsync(int taskId)
    {
        try
        {
            var taskEntity = await _context.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                return null; // Task not found
            }

            var taskModel = _mapper.Map<TodoTask>(taskEntity);
            taskModel.IsOverdue = taskModel.DueDate.HasValue && taskModel.DueDate < DateTime.Now;
            return taskModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving task with ID {taskId}.", taskId);
            throw; // Rethrow the exception to be handled in the controller
        }
    }

    public async Task<TodoTask> AddTaskAsync(TaskEntity taskEntity)
    {
        // 1. Validate TodoListId:
        var todoListExists = await _context.TodoLists.AnyAsync(tl => tl.Id == taskEntity.TodoListId);

        if (!todoListExists)
        {
            throw new ArgumentException($"Todo list with ID {taskEntity.TodoListId} not found.");
        }

        // 2. Other Validations (Optional):
        // - You might have additional validation rules for your task, such as:
        //   - Required fields (e.g., Title)
        //   - Maximum length constraints
        //   - Custom business logic

        // 3. Set Additional Properties (if needed):
        taskEntity.CreatedDate = DateTime.UtcNow; // Set the current UTC time

        // 4. Add to Database:
        _context.Tasks.Add(taskEntity);
        await _context.SaveChangesAsync();

        // 5. Logging (Optional):
        _logger.LogInformation("Created task with ID {TaskId} for user {UserId}", taskEntity.Id, taskEntity.UserId);

        // 6. Return Result:
        return _mapper.Map<TodoTask>(taskEntity); // Map back to TodoTask using AutoMapper
    }


    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        try
        {
            var taskEntity = await _context.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                return false; // Task not found
            }

            _context.Tasks.Remove(taskEntity);
            await _context.SaveChangesAsync();

            return true; // Task deleted successfully
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while deleting task with ID {taskId}.", taskId);
            throw new Exception("An error occurred while deleting the task.", ex);
        }
    }

    public async Task<TodoTask> UpdateTaskAsync(int taskId, TodoTask updatedTodoTask)
    {
        try
        {
            // 1. Check if the Task exists
            var existingTask = await _context.Tasks.FindAsync(taskId);
            if (existingTask == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found.");
            }

            // 2. Check concurrency token

            // 3. Map the updated TodoTask to TaskEntity
            _mapper.Map(updatedTodoTask, existingTask); // Use AutoMapper

            // 4. Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<TodoTask>(existingTask);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Reload the task to get the latest version
                _context.Entry(existingTask).Reload();

                // Map the updated values (except RowVersion) to the reloaded entity
                _mapper.Map(updatedTodoTask, existingTask);

                // Retry saving changes
                await _context.SaveChangesAsync();

                return _mapper.Map<TodoTask>(existingTask);
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid input while updating a task.");
            throw; // Rethrow the ArgumentException to be handled by the controller.
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while updating the task in the database.");
            throw new Exception("An error occurred while updating the task.", ex); // Throw a more general exception with a custom message.
        }
    }

    public async Task<TodoTask> UpdateTaskStatusAsync(int todoListId, int taskId, ToDoTaskStatus newStatus)
    {
        // 1. Get the user ID from the token claims (same as in your TodoListController)

        // 2. Retrieve the task from the database
        var taskEntity = await _context.Tasks
            .Where(t => t.TodoListId == todoListId && t.Id == taskId)
            .FirstOrDefaultAsync();

        // 3. Authorization check: Verify if the user owns the task
        // ... (same as before)

        // 4. Update the task status and save changes
        taskEntity.Status = newStatus;
        await _context.SaveChangesAsync();
        return _mapper.Map<TodoTask>(taskEntity); // Use AutoMapper to map
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

    public async Task<IEnumerable<TodoTask>> SearchByTitleAsync(string title)
    {
        var tasks = await _context.Tasks
            .Where(t => EF.Functions.Like(t.Title, $"%{title}%"))
            .ToListAsync();

        return _mapper.Map<IEnumerable<TodoTask>>(tasks);
    }
}
