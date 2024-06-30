using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;


[ApiController]
[Route("api/task")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;
    private readonly IMapper _mapper;
    private readonly TodoListDbContext _context;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger, IMapper mapper, TodoListDbContext context)
    {
        this._taskService = taskService;
        this._logger = logger;
        this._mapper = mapper;
        this._context = context;
    }


    [HttpGet("{todoListId}/tasks")]
    [Authorize(Roles = "Owner,Editor,Viewer")]
    public async Task<ActionResult<List<TodoTask>>> GetTasks(int todoListId)
    {
        var tasks = await this._taskService.GetTasksForTodoListAsync(todoListId);
        return this.Ok(tasks);
    }

    [HttpPost("{todoListId}/tasks")] // POST /api/tasks/{todoListId}/tasks
    [Authorize(Roles = "Owner,Editor")]
    public async Task<ActionResult<TodoTask>> AddTask(int todoListId, [FromBody] TodoTask newTodoTask)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return this.Unauthorized("Invalid token: Missing User ID claim.");
            }

            var taskEntity = new TaskEntity
            {
                Title = newTodoTask.Title,
                Description = newTodoTask.Description,
                TodoListId = todoListId,
                CreatedDate = DateTime.Now,
                Status = ToDoTaskStatus.NotStarted, // Assuming you have an enum for task status
                UserId = userIdClaim.Value // Set the UserId on the entity
            };

            var createdTask = await this._taskService.AddTaskAsync(taskEntity); // Pass the entity to your service

            if (createdTask == null)
            {
                return this.NotFound(); // Or BadRequest if you want to signal an error
            }
            else
            {
                return this.CreatedAtAction(nameof(this.GetTasks), new { todoListId = createdTask.TodoListId }, createdTask);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("{taskId}")]
    [Authorize(Roles = "Owner,Editor,Viewer")]
    public async Task<ActionResult<TodoTaskDto>> GetTaskById(int taskId)
    {
        try
        {
            var task = await this._taskService.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return this.NotFound("Task not found.");
            }

            var taskDto = this._mapper.Map<TodoTaskDto>(task);
            taskDto.IsOverdue = task.IsOverdue;

            if (User != null)
            {
                var userTimeZoneId = User.FindFirstValue("TimeZone") ?? "UTC";
                var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);

                try
                {
                    taskDto.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(taskDto.CreatedDate, userTimeZone);
                    taskDto.DueDate = taskDto.DueDate.HasValue
                        ? TimeZoneInfo.ConvertTimeFromUtc(taskDto.DueDate.Value, userTimeZone)
                        : null;
                }
                catch (TimeZoneNotFoundException ex)
                {
                }
            }

            return this.Ok(taskDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    // TaskController.cs
    [HttpPut("{taskId}")] // PUT /api/tasks/{taskId}
    [Authorize(Roles = "Owner,Editor")]
    public async Task<ActionResult<TodoTaskDto>> UpdateTask(int taskId, [FromBody] TodoTask updatedTodoTask)
    {
        if (taskId != updatedTodoTask.Id)
        {
            return this.BadRequest("Task ID mismatch");
        }

        try
        {
            var updated = await _taskService.UpdateTaskAsync(taskId, updatedTodoTask);
            if (updated == null)
            {
                return this.NotFound();
            }

            return this.Ok(updated);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return this.Conflict("The task has been updated by someone else. Please refresh and try again.");
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{taskId}")]
    [Authorize(Roles = "Owner,Editor")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        try
        {
            var wasDeleted = await this._taskService.DeleteTaskAsync(taskId);
            if (!wasDeleted)
            {
                return this.NotFound(); // Return 404 if the task doesn't exist
            }

            return this.NoContent(); // Return 204 No Content on successful deletion
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, "Internal server error");
        }
    }

    // In your Web API TaskController
    [HttpPut("{taskId}/status")]
    [Authorize(Roles = "Owner,Editor")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] UpdateTaskStatusRequest request)
    {
        // Validate the request
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }
        try
        {
            // Update the task status using your _taskService
            var updatedTask = await this._taskService.UpdateTaskStatusAsync(request.TodoListId, taskId, request.NewStatus);
            if (updatedTask == null)
            {
                return this.NotFound();
            }

            var updatedTaskDto = _mapper.Map<TodoTaskDto>(updatedTask);
            return this.Ok(updatedTaskDto);
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, "Internal server error");
        }
    }



    [HttpGet("GetMyTasks")]
    [Authorize]
    public IActionResult GetMyTasks([FromQuery] ToDoTaskStatus? status = null, [FromQuery] string sortBy = "Name", [FromQuery] string sortOrder = "asc")
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return this.Unauthorized("Invalid token: Missing User ID claim.");
        }

        var userId = userIdClaim.Value; // Remove the duplicate declaration of 'string userId'

        var tasks = this._context.Tasks.Where(t => t.UserId == userId).ToList();

        if (status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == status).ToList();
        }

        // Apply sorting
        switch (sortBy.ToLower())
        {
            case "name":
                tasks = sortOrder == "desc" ? tasks.OrderByDescending(t => t.Title).ToList() : tasks.OrderBy(t => t.Title).ToList();
                break;
            case "duedate":
                tasks = sortOrder == "desc" ? tasks.OrderByDescending(t => t.DueDate).ToList() : tasks.OrderBy(t => t.DueDate).ToList();
                break;
            default:
                // Default sorting (by Name ascending)
                tasks = tasks.OrderBy(t => t.Title).ToList();
                break;
        }
        var taskDtos = this._mapper.Map<List<TodoTaskDto>>(tasks);
        return Ok(taskDtos);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> Search([FromQuery] string title)
    {
        try
        {
            var tasks = await this._taskService.SearchByTitleAsync(title);
            var taskDtos = this._mapper.Map<IEnumerable<TodoTaskDto>>(tasks);

            if (!taskDtos.Any())
            {
                return this.NotFound("No tasks found matching the search criteria.");
            }

            return this.Ok(taskDtos);
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, "Internal server error");
        }
    }
}
