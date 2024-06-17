
namespace TodoListApp.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApi.Models;

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


    public async Task<List<TaskModel>> GetTasksForTodoListAsync(int todoListId)
    {
        try
        {
            var taskEntities = await _context.Tasks
                .Where(t => t.TodoListId == todoListId)
                .ToListAsync();

            var taskModels = _mapper.Map<List<TaskModel>>(taskEntities);

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

    public async Task<TaskModel> GetTaskByIdAsync(int taskId)
    {
        try
        {
            var taskEntity = await _context.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                return null; // Task not found
            }

            var taskModel = _mapper.Map<TaskModel>(taskEntity);
            taskModel.IsOverdue = taskModel.DueDate.HasValue && taskModel.DueDate < DateTime.Now;
            return taskModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving task with ID {taskId}.", taskId);
            throw; // Rethrow the exception to be handled in the controller
        }
    }

    public async Task<TaskModel> AddTaskAsync(TaskEntity taskEntity)
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
        return _mapper.Map<TaskModel>(taskEntity); // Map back to TaskModel using AutoMapper
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

    public async Task<TaskModel> UpdateTaskAsync(int taskId, TaskModel updatedTask)
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

            // 3. Map the updated TaskModel to TaskEntity
            _mapper.Map(updatedTask, existingTask); // Use AutoMapper

            // 4. Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<TaskModel>(existingTask);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Reload the task to get the latest version
                _context.Entry(existingTask).Reload();

                // Map the updated values (except RowVersion) to the reloaded entity
                _mapper.Map(updatedTask, existingTask);

                // Retry saving changes
                await _context.SaveChangesAsync();

                return _mapper.Map<TaskModel>(existingTask);
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

    public async Task<TaskModel> UpdateTaskStatusAsync(int todoListId, int taskId, ToDoTaskStatus newStatus)
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

        // 5. Return the updated task (optional)
        return _mapper.Map<TaskModel>(taskEntity);
    }
}
