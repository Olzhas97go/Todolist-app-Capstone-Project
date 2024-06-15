namespace TodoListApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Tasks;
using System.Security.Claims;
using TodoListApp.WebApi.Models;

[ApiController]
[Route("api/task")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }


    [HttpGet("{todoListId}/tasks")]
    public async Task<ActionResult<List<TaskModel>>> GetTasks(int todoListId)
    {
        var tasks = await _taskService.GetTasksForTodoListAsync(todoListId);
        return Ok(tasks);
    }

    [HttpPost("{todoListId}/tasks")] // POST /api/tasks/{todoListId}/tasks
    [Authorize]
    public async Task<ActionResult<TaskModel>> AddTask(int todoListId, [FromBody] TaskModel newTask)
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
                Title = newTask.Title,
                Description = newTask.Description,
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


    [HttpGet("{taskId}")]  // GET /api/tasks/{taskId}
    public async Task<ActionResult<TaskModel>> GetTaskById(int taskId)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound(); // Task not found
            }
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving task with ID {taskId}.", taskId);
            return StatusCode(500, "Internal server error");
        }
    }

    // TaskController.cs
    [HttpPut("{taskId}")]  // PUT /api/tasks/{taskId}
    public async Task<ActionResult<TaskModel>> UpdateTask(int taskId, [FromBody] TaskModel updatedTask)
    {
        if (taskId != updatedTask.Id)
        {
            return BadRequest("Task ID mismatch");
        }

        try
        {
            var updated = await _taskService.UpdateTaskAsync(taskId, updatedTask);
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

    [HttpGet("TestAuth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        return Ok("Authenticated!");
    }
}
