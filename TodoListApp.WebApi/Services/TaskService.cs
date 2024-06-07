namespace TodoListApp.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.WebApi.DataContext;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

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

    public async Task<TaskModel> AddTaskAsync(int todoListId, TaskModel newTask)
    {
        try
        {
            // 1. Check if the TodoList exists
            var todoList = await _context.TodoLists.FindAsync(todoListId);
            if (todoList == null)
            {
                throw new ArgumentException($"Todo list with ID {todoListId} not found.");
            }

            // 2. Map the TaskModel to TaskEntity
            var taskEntity = _mapper.Map<TaskEntity>(newTask);
            taskEntity.TodoListId = todoListId;
            taskEntity.TodoList = todoList; // Optionally, explicitly set the navigation property

            // 3. Add to the database and save changes
            _context.Tasks.Add(taskEntity);
            await _context.SaveChangesAsync();

            // 4. Map the newly created TaskEntity back to TaskModel and return
            return _mapper.Map<TaskModel>(taskEntity);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid input while creating a task.");
            throw; // Rethrow the ArgumentException to be handled by the controller.
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while saving the task to the database.");
            throw new Exception("An error occurred while saving the task.", ex); // Throw a more general exception with a custom message.
        }
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
}
