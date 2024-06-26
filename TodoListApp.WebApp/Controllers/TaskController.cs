using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Refit;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.TaskModels;
using AppTask = TodoListApp.WebApp.Models.TaskModels.Task;

namespace TodoListApp.WebApp.Controllers;

public class TaskController : Controller
{
    private readonly ITodoListApi _todoListApi; // Refit interface
    private readonly IMapper _mapper;
    private readonly ILogger<TodoListController> _logger;
    private readonly ITaskService _taskService;

    public TaskController(ITodoListApi todoListApi, IMapper mapper, ILogger<TodoListController> logger)
    {
        _logger = logger;
        _todoListApi = todoListApi;
        _mapper = mapper;
    }

    [HttpGet("{todoListId}/tasks")]
    public async Task<IActionResult> ViewTasks(int todoListId)
    {
        try
        {
            var todoList = await _todoListApi.GetTodoListById(todoListId);
            if (todoList == null)
            {
                return NotFound("Todo list not found.");
            }

            var tasks = await _todoListApi.GetTasksForTodoListAsync(todoListId); // Gets a list of TodoTask objects

            var todoListModels = tasks.Select(t => new TodoListModel
            {
                Id = t.Id,
                Name = t.Title,    // Map Title to Name
                IsCompleted = t.IsCompleted, // Add IsCompleted property to TodoListModel
                IsOverdue = t.DueDate < DateTime.Now  // Add IsOverdue property to TodoListModel
            }).ToList();

            var viewModel = new TodoListWithTasksViewModel
            {
                TodoList = todoList,
                Tasks = todoListModels // Now uses TodoListModel
            };

            return View("ViewTasks", viewModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Identity/Account/Login?ReturnUrl=%2F");
            }
            _logger.LogError(ex, "API error while fetching todo list details.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    [HttpGet("{taskId}")]
    public async Task<IActionResult> Details(int taskId)
    {
        try
        {
            var taskDto = await _todoListApi.GetTaskByIdAsync(taskId);
            if (taskDto == null)
            {
                return NotFound("Task not found.");
            }

            var todoListDto = await _todoListApi.GetTodoListById(taskDto.TodoListId);
            if (todoListDto == null)
            {
                return NotFound("Todo list associated with task not found.");
            }

            // Create an empty list to store tasks
            var taskList = new List<TodoTaskDto>();

            // Fetch all tasks using the TaskIds and add them to the list
            foreach (var id in todoListDto.TaskIds)
            {
                taskDto = await _todoListApi.GetTaskByIdAsync(id);
                if (taskDto != null)
                {
                    var taskDtoMapped = _mapper.Map<TodoTaskDto>(taskDto);
                    taskList.Add(taskDtoMapped);
                }
            }

            // Filter to the SelectedTask by taskId
            var selectedTaskDto = taskList.SingleOrDefault(x => x.Id == taskId);
            var selectedTask = _mapper.Map<TodoTask>(selectedTaskDto);
            var taskViewModel = _mapper.Map<TodoTask>(selectedTaskDto);

            var todoListWebApiModel = new TodoListWebApiModel
            {
                Id = todoListDto.Id,
                Name = todoListDto.Name,
                Description = todoListDto.Description,
                Tasks = taskList // Update the list of tasks
            };

            var viewModel = new TaskDetailsViewModel
            {
                SelectedTask = taskViewModel,
                TodoList = _mapper.Map<TodoListDto>(todoListWebApiModel) // Map to TodoListDto
            };

            return View("TaskDetails", viewModel);
        }
        catch (ApiException ex)
        {
            // Handle API exceptions
            _logger.LogError(ex, "API error fetching task details: {StatusCode}",
                ex.StatusCode); // Use the injected logger
            return StatusCode((int)ex.StatusCode);
        }
        catch (DbUpdateException ex)
        {
            // Handle database update exceptions
            this._logger.LogError(ex, "Error updating database");
            return StatusCode(500); // Internal Server Error
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions and rethrow
            this._logger.LogError(ex, "An unexpected error occurred.");
            throw; // Rethrow the exception
        }
    }

    [HttpGet("{todoListId}/CreateTask")]
    public async Task<IActionResult> CreateTask(int todoListId)
    {
        try
        {
            var todoListDto = await _todoListApi.GetTodoListById(todoListId);
            if (todoListDto == null)
            {
                return NotFound("To-do list not found.");
            }

            var viewModel = new CreateTaskViewModel
            {
                TodoListId = todoListId, TodoListName = todoListDto.Name, DueDate = DateTime.Today
            };

            return View(viewModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Unauthorized access - redirect to login or handle as needed
                return Redirect("/Identity/Account/Login?ReturnUrl=%2F");
            }
            _logger.LogError(ex, "API error while fetching todo list for creating a task: {StatusCode}", ex.StatusCode);
            return StatusCode((int)ex.StatusCode);
        }
    }


    [HttpPost("{todoListId}/CreateTask")]
    public async Task<IActionResult> CreateTask(int todoListId, CreateTaskViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var todoListDto = await _todoListApi.GetTodoListById(todoListId);
            if (todoListDto != null)
            {
                viewModel.TodoListName = todoListDto.Name;
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while fetching the to-do list.");
            }

            //return View(viewModel);
        }

        try
        {
            var todoTaskDto = _mapper.Map<TodoTaskDto>(viewModel);

            // Get the user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            todoTaskDto.TodoListId = todoListId; // Set the todoListId

            // Create the task on the API
            var newTask = await _todoListApi.AddTask(todoListId, todoTaskDto);

            return RedirectToAction("ViewTasks", new { todoListId = viewModel.TodoListId });
        }
        catch (Exception ex)
        {
            if (ex is ApiException apiEx) // Check if it's an ApiException
            {
                _logger.LogError(apiEx, "API error while creating task: {StatusCode} - {Message}", apiEx.StatusCode, apiEx.Message);

                if (apiEx.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorContent = await apiEx.GetContentAsAsync<ValidationProblemDetails>();
                    foreach (var error in errorContent.Errors)
                    {
                        ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the task.");
                }

                // ... (re-fetch todoListDto and return View - same as before)
            }
            else if (ex is DbUpdateException dbEx)
            {
                // ... (database error handling - same as before)
            }
            else // Catch-all for unexpected errors
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the task.");
                throw; // Rethrow the exception to be handled globally
            }

            return View(viewModel); // Return the view if there was any error
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int taskId)
    {
        try
        {
            // 1. Verify Authorization (JWT Token)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                _logger.LogWarning("Missing User ID claim in JWT token.");
                return Unauthorized("Unauthorized access.");
            }

            // 2. Verify Ownership (Implement in your service layer)
            var task = await _todoListApi.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            var todoListId = task.TodoListId;

            // 3. Delete the Task
            await _todoListApi.DeleteTask(taskId);

            // 4. Redirect back to the ViewTasks page for the corresponding TodoList
            return RedirectToAction("ViewTasks", new { todoListId });
        }
        catch (ApiException ex)
        {
            // Handle API-related exceptions
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                // Unauthorized to delete
                TempData["ErrorMessage"] = "You do not have permission to delete this task.";
                return RedirectToAction("AccessDenied", "Error");
            }
            else if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Task not found
                TempData["ErrorMessage"] = "Task not found.";
                return RedirectToAction("Index", "TodoList");
            }
            else
            {
                // General error
                _logger.LogError(ex, "An error occurred while deleting the task.");
                TempData["ErrorMessage"] = "An error occurred while deleting the task.";
                return RedirectToAction("Index", "TodoList");
            }
        }
    }

    [HttpGet("{taskId}/edit")]
    public async Task<IActionResult> EditTask(int taskId)
    {
        try
        {
            var taskDto = await _todoListApi.GetTaskByIdAsync(taskId);
            if (taskDto == null)
            {
                return NotFound("Task not found.");
            }

            var taskViewModel = _mapper.Map<TodoListApp.WebApp.Models.TaskModels.Task>(taskDto);
            return View(taskViewModel); // Pass the correct view model type
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, message: "API error fetching task details.");
            return StatusCode((int)ex.StatusCode);
        }
    }

// Update the EditTask Post Method
    [HttpPost("{taskId}/edit")]
    public async Task<IActionResult> EditTask(int taskId, TodoListApp.WebApp.Models.TaskModels.Task updatedTask)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedTask); // Redisplay form with validation errors
        }

        try
        {
            var todoTaskDto = _mapper.Map<TodoTaskDto>(updatedTask);
            todoTaskDto.TodoListId = taskId;
            var updatedDto = await _todoListApi.UpdateTask(updatedTask.Id, todoTaskDto);
            if(updatedDto is null)
            {
                return this.NotFound();
            }

            return RedirectToAction("Details", "Task", new { taskId = updatedTask.Id });
        }
        catch (ApiException ex)
        {
            // Handle API exceptions (401 Unauthorized, 404 Not Found, others)
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this task.";
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                _logger.LogError(ex, "API error while updating task: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
                ModelState.AddModelError("", "An error occurred while updating the task. Please try again later.");

                return View(updatedTask); // Return the same view model type as in HttpGet
            }
        }
    }

    // In TaskController.cs
    [HttpGet("MyTasks")]
    public async Task<IActionResult> MyTasks(ToDoTaskStatus? status = null, string sortBy = "Name", string sortOrder = "asc")
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: Missing User ID claim.");
            }
            string userId = userIdClaim.Value;

            // Fetch tasks using Refit
            var tasks = await _todoListApi.GetMyTasks(userId, status, sortBy, sortOrder);

            var taskListModels = _mapper.Map<List<TodoListModel>>(tasks);


            var viewModel = new TodoListWithTasksViewModel
            {
                UserId = userId,
                Tasks = taskListModels, // Use the manually mapped list
                StatusFilter = status,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }
        catch (ApiException ex)
        {
            // Handle API errors (e.g., not found, unauthorized, etc.)
            _logger.LogError(ex, "API Error while fetching tasks.");
            return StatusCode((int)ex.StatusCode, "API Error: " + ex.Message);
        }
    }
}
