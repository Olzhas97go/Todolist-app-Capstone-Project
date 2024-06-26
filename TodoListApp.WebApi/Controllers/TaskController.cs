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
        _taskService = taskService;
        _logger = logger;
        _mapper = mapper;
        _context = context;
    }


    [HttpGet("{todoListId}/tasks")]
    public async Task<ActionResult<List<TodoTask>>> GetTasks(int todoListId)
    {
        var tasks = await _taskService.GetTasksForTodoListAsync(todoListId);
        return Ok(tasks);
    }

    [HttpPost("{todoListId}/tasks")] // POST /api/tasks/{todoListId}/tasks
    [Authorize]
    public async Task<ActionResult<TodoTask>> AddTask(int todoListId, [FromBody] TodoTask newTodoTask)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: Missing User ID claim.");
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

            var createdTask = await _taskService.AddTaskAsync(taskEntity); // Pass the entity to your service

            if (createdTask == null)
            {
                return NotFound(); // Or BadRequest if you want to signal an error
            }
            else
            {
                return CreatedAtAction(nameof(GetTasks), new { todoListId = createdTask.TodoListId }, createdTask);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a task for todo list with ID {todoListId}.", todoListId);
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("{taskId}")]
    public async Task<ActionResult<TodoTaskDto>> GetTaskById(int taskId)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound(); // Task not found
            }
            var taskDto = _mapper.Map<TodoTaskDto>(task);
            taskDto.IsOverdue = task.IsOverdue;  // Map the calculated IsOverdue property

            return Ok(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving task with ID {taskId}.", taskId);
            return StatusCode(500, "Internal server error");
        }
    }


    // TaskController.cs
    [HttpPut("{taskId}")]  // PUT /api/tasks/{taskId}
    public async Task<ActionResult<TodoTaskDto>> UpdateTask(int taskId, [FromBody] TodoTask updatedTodoTask)
    {
        if (taskId != updatedTodoTask.Id)
        {
            return BadRequest("Task ID mismatch");
        }

        try
        {
            var todoTask = _mapper.Map<TodoTask>(updatedTodoTask);
            var updated = await _taskService.UpdateTaskAsync(taskId, updatedTodoTask);
            if (updated == null)
            {
                return NotFound(); // Task not found
            }

            return Ok(updated);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict("The task has been updated by someone else. Please refresh and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating task with ID {taskId}.", taskId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        try
        {
            var wasDeleted = await _taskService.DeleteTaskAsync(taskId);
            if (!wasDeleted)
            {
                return NotFound(); // Return 404 if the task doesn't exist
            }

            return NoContent(); // Return 204 No Content on successful deletion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting task with ID {taskId}.", taskId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{todoListId}/{taskId}/status")] // PUT /api/Task/{todoListId}/{taskId}/status
    [Authorize]
    public async Task<IActionResult> UpdateTaskStatus(int todoListId, int taskId, ToDoTaskStatus newStatus)
    {
        var updatedTask = await _taskService.UpdateTaskStatusAsync(todoListId, taskId, newStatus);

        if (updatedTask == null)
        {
            return NotFound("Task not found.");
        }

        return Ok(updatedTask);
    }

    [HttpGet("GetMyTasks")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetMyTasks([FromQuery] ToDoTaskStatus? status = null, [FromQuery] string sortBy = "Name", [FromQuery] string sortOrder = "asc")
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("Invalid token: Missing User ID claim.");
        }

        var userId = userIdClaim.Value; // Remove the duplicate declaration of 'string userId'

        var tasks = _context.Tasks.Where(t => t.UserId == userId).ToList();

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

        return Ok(tasks);
    }
}
