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
        this._context = context;
        this._mapper = mapper;
        this._logger = logger;
    }


    public async Task<List<TodoTask>> GetTasksForTodoListAsync(int todoListId)
    {
        try
        {
            var taskEntities = await this._context.Tasks
                .Where(t => t.TodoListId == todoListId)
                .ToListAsync();

            var taskModels = this._mapper.Map<List<TodoTask>>(taskEntities);

            foreach (var task in taskModels)
            {
                task.IsOverdue = task.DueDate.HasValue && task.DueDate.Value < DateTime.Now;
            }

            return taskModels;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<TodoTask> GetTaskByIdAsync(int taskId)
    {
        try
        {
            var taskEntity = await this._context.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                return null;
            }

            var taskModel = this._mapper.Map<TodoTask>(taskEntity);
            taskModel.IsOverdue = taskModel.DueDate.HasValue && taskModel.DueDate < DateTime.Now;
            return taskModel;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<TodoTask> AddTaskAsync(TaskEntity taskEntity)
    {
        var todoListExists = await this._context.TodoLists.AnyAsync(tl => tl.Id == taskEntity.TodoListId);

        if (!todoListExists)
        {
            throw new ArgumentException($"Todo list with ID {taskEntity.TodoListId} not found.");
        }

        taskEntity.CreatedDate = DateTime.UtcNow;

        this._context.Tasks.Add(taskEntity);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<TodoTask>(taskEntity);
    }


    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        try
        {
            var taskEntity = await this._context.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                return false;
            }

            this._context.Tasks.Remove(taskEntity);
            await this._context.SaveChangesAsync();

            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while deleting the task.", ex);
        }
    }

    public async Task<TodoTask> UpdateTaskAsync(int taskId, TodoTask updatedTodoTask)
    {
        try
        {
            var existingTask = await this._context.Tasks.FindAsync(taskId);
            if (existingTask == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found.");
            }

            this._mapper.Map(updatedTodoTask, existingTask);

            try
            {
                await this._context.SaveChangesAsync();
                return this._mapper.Map<TodoTask>(existingTask);
            }
            catch (DbUpdateConcurrencyException)
            {
                this._context.Entry(existingTask).Reload();

                this._mapper.Map(updatedTodoTask, existingTask);

                await this._context.SaveChangesAsync();

                return this._mapper.Map<TodoTask>(existingTask);
            }
        }
        catch (ArgumentException ex)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while updating the task.", ex);
        }
    }

    public async Task<TodoTask> UpdateTaskStatusAsync(int todoListId, int taskId, ToDoTaskStatus newStatus)
    {
        var taskEntity = await this._context.Tasks
            .Where(t => t.TodoListId == todoListId && t.Id == taskId)
            .FirstOrDefaultAsync();

        taskEntity.Status = newStatus;
        await this._context.SaveChangesAsync();
        return this._mapper.Map<TodoTask>(taskEntity);
    }

    public List<TodoListModel> GetTasksForUser(string userId, ToDoTaskStatus? status = null, string sortBy = "Name", string sortOrder = "asc")
    {
        var query = this._context.Tasks.Where(t => t.UserId == userId);

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
                query = query.OrderBy(t => t.Title);
                break;
        }

        return query
            .Select(t => new TodoListModel { Id = t.Id, Name = t.Title, Description = t.Description, Status = t.Status })
            .ToList();
    }

    public async Task<IEnumerable<TodoTask>> SearchByTitleAsync(string title)
    {
        var tasks = await this._context.Tasks
            .Where(t => EF.Functions.Like(t.Title, $"%{title}%"))
            .ToListAsync();

        return this._mapper.Map<IEnumerable<TodoTask>>(tasks);
    }
}
