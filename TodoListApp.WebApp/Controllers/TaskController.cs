using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.TaskModels;

namespace TodoListApp.WebApp.Controllers;

public class TaskController : Controller
{
    private readonly ITodoListApi _todoListApi;
    private readonly IMapper _mapper;
    private readonly ILogger<TodoListController> _logger;
    private readonly ITaskService _taskService;

    public TaskController(ITodoListApi todoListApi, IMapper mapper, ILogger<TodoListController> logger)
    {
        this._logger = logger;
        this._todoListApi = todoListApi;
        this._mapper = mapper;
    }

    [HttpGet("{todoListId}/tasks")]
    public async Task<IActionResult> ViewTasks(int todoListId)
    {
        try
        {
            var todoList = await this._todoListApi.GetTodoListById(todoListId);
            if (todoList == null)
            {
                return this.NotFound("Todo list not found.");
            }

            var tasks = await this._todoListApi.GetTasksForTodoListAsync(todoListId);

            var todoListModels = tasks.Select(t => new TodoListModel
            {
                Id = t.Id,
                Name = t.Title,
                IsCompleted = t.IsCompleted,
                IsOverdue = t.DueDate < DateTime.Now,
            }).ToList();

            var viewModel = new TodoListWithTasksViewModel
            {
                TodoList = todoList,
                Tasks = todoListModels,
            };

            return this.View("ViewTasks", viewModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return this.Redirect("/Identity/Account/Login?ReturnUrl=%2F");
            }

            return this.View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> Details(int taskId)
    {
    try
    {
        var taskDto = await this._todoListApi.GetTaskByIdAsync(taskId);
        if (taskDto == null)
        {
            return this.NotFound("Task not found.");
        }

        var todoListDto = await this._todoListApi.GetTodoListById(taskDto.TodoListId);
        if (todoListDto == null)
        {
            return this.NotFound("Todo list associated with task not found.");
        }

        var allTasksDto = await this._todoListApi.GetTasksForTodoListAsync(todoListDto.Id);
        var tagDtos = await this._todoListApi.GetTagsForTaskAsync(taskId);
        var selectedTask = this._mapper.Map<TodoTask>(taskDto);
        var allTasks = this._mapper.Map<List<TodoTaskDto>>(allTasksDto);

        var viewModel = new TaskDetailsViewModel
        {
            SelectedTask = selectedTask,
            TodoList = this._mapper.Map<TodoListDto>(todoListDto),
            AllTasks = allTasks,
            Tags = this._mapper.Map<List<TagDto>>(tagDtos),
        };

        return this.View("TaskDetails", viewModel);
    }
    catch (ApiException apiEx)
    {
        this._logger.LogError(apiEx, "API error fetching task details: {StatusCode}", apiEx.StatusCode);

        if (apiEx.StatusCode == HttpStatusCode.NotFound)
        {
            return this.NotFound("Task or related data not found.");
        }
        else
        {
            return this.StatusCode((int)apiEx.StatusCode, "API error occurred while retrieving task details.");
        }
    }
    catch (DbUpdateException ex)
    {
        this._logger.LogError(ex, "Error updating database");
        return this.StatusCode(500); // Internal Server Error
    }
    catch (Exception ex)
    {
        this._logger.LogError(ex, "An unexpected error occurred.");
        throw;
    }
    }

    [HttpGet("{todoListId}/CreateTask")]
    public async Task<IActionResult> CreateTask(int todoListId)
    {
        try
        {
            var todoListDto = await this._todoListApi.GetTodoListById(todoListId);
            if (todoListDto == null)
            {
                return this.NotFound("To-do list not found.");
            }

            var viewModel = new CreateTaskViewModel
            {
                TodoListId = todoListId, TodoListName = todoListDto.Name, DueDate = DateTime.Today,
            };

            return this.View(viewModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return this.Redirect("/Identity/Account/Login?ReturnUrl=%2F");
            }

            return this.StatusCode((int)ex.StatusCode);
        }
    }

    [HttpPost("{todoListId}/CreateTask")]
    public async Task<IActionResult> CreateTask(int todoListId, CreateTaskViewModel viewModel)
    {
        if (!this.ModelState.IsValid)
        {
            var todoListDto = await this._todoListApi.GetTodoListById(todoListId);
            if (todoListDto != null)
            {
                viewModel.TodoListName = todoListDto.Name;
            }
            else
            {
                this.ModelState.AddModelError(" ", "An error occurred while fetching the to-do list.");
            }
        }

        try
        {
            var todoTaskDto = this._mapper.Map<TodoTaskDto>(viewModel);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            todoTaskDto.TodoListId = todoListId; // Set the todoListId

            var newTask = await this._todoListApi.AddTask(todoListId, todoTaskDto);

            return this.RedirectToAction("ViewTasks", new { todoListId = viewModel.TodoListId });
        }
        catch (Exception ex)
        {
            if (ex is ApiException apiEx)
            {
                this._logger.LogError(apiEx, "API error while creating task: {StatusCode} - {Message}", apiEx.StatusCode, apiEx.Message);

                if (apiEx.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorContent = await apiEx.GetContentAsAsync<ValidationProblemDetails>();
                    foreach (var error in errorContent.Errors)
                    {
                        this.ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "An error occurred while creating the task.");
                }
            }
            else if (ex is DbUpdateException dbEx)
            {
            }
            else
            {
                this._logger.LogError(ex, "An unexpected error occurred while creating the task.");
                throw;
            }

            return this.View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int taskId)
    {
        try
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return this.Unauthorized("Unauthorized access.");
            }

            var task = await this._todoListApi.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return this.NotFound("Task not found.");
            }

            var todoListId = task.TodoListId;

            await this._todoListApi.DeleteTask(taskId);

            return this.RedirectToAction("ViewTasks", new { todoListId });
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                this.TempData["ErrorMessage"] = "You do not have permission to delete this task.";
                return this.RedirectToAction("AccessDenied", "Error");
            }
            else if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Task not found
                this.TempData["ErrorMessage"] = "Task not found.";
                return this.RedirectToAction("Index", "TodoList");
            }
            else
            {
                this.TempData["ErrorMessage"] = "An error occurred while deleting the task.";
                return this.RedirectToAction("Index", "TodoList");
            }
        }
    }

    [HttpGet("{taskId}/edit")]
    public async Task<IActionResult> EditTask(int taskId)
    {
        try
        {
            var taskDto = await this._todoListApi.GetTaskByIdAsync(taskId);
            if (taskDto == null)
            {
                return this.NotFound("Task not found.");
            }

            var taskViewModel = this._mapper.Map<TodoListApp.WebApp.Models.TaskModels.Task>(taskDto);
            return this.View(taskViewModel);
        }
        catch (ApiException ex)
        {
            return this.StatusCode((int)ex.StatusCode);
        }
    }

    [HttpPost("{taskId}/edit")]
    public async Task<IActionResult> EditTask([FromQuery] int taskId, TodoListApp.WebApp.Models.TaskModels.Task updatedTask)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(updatedTask);
        }

        try
        {
            var todoTaskDto = this._mapper.Map<TodoTaskDto>(updatedTask);
            todoTaskDto.TodoListId = taskId;
            todoTaskDto.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedDto = await this._todoListApi.UpdateTask(updatedTask.Id, todoTaskDto);
            if (updatedDto is null)
            {
                return this.NotFound();
            }

            this.TempData["ShowBackToTasksLink"] = true;
            return this.RedirectToAction("Details", "Task", new { taskId = updatedTask.Id });
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                return this.RedirectToAction("AccessDenied", "Error");
            }
            else if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else
            {
                return this.RedirectToAction("Error", "Home");
            }
        }
    }

    [HttpGet("MyTasks")]
    public async Task<IActionResult> MyTasks(string searchString = "", string status = null, string sortBy = "Name", string sortOrder = "asc")
    {
        try
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return this.Unauthorized("Invalid token: Missing User ID claim.");
            }

            string userId = userIdClaim.Value;
            IEnumerable<TodoTask> tasks;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var searchedTasks = await this._todoListApi.Search(searchString);
                tasks = this._mapper.Map<IEnumerable<TodoTask>>(searchedTasks);
            }
            else
            {
                tasks = this._mapper.Map<IEnumerable<TodoTask>>(await this._todoListApi.GetMyTasks(userId, status, sortBy, sortOrder)); // Map to TodoTask
            }

            var taskListModels = this._mapper.Map<List<TodoListModel>>(tasks);

            var viewModel = new TodoListWithTasksViewModel
            {
                UserId = userId,
                Tasks = taskListModels,
                StatusFilter = status != null ? Enum.Parse<ToDoTaskStatus>(status) : null,
                SortBy = sortBy,
                SortOrder = sortOrder,
                SearchString = searchString,
            };

            return this.View(viewModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationErrors = await ex.GetContentAsAsync<ValidationProblemDetails>();
                foreach (var error in validationErrors.Errors)
                {
                    this.ModelState.AddModelError(error.Key, error.Value.First());
                }

                return this.View("MyTasks");
            }

            return this.StatusCode((int)ex.StatusCode, "API Error: " + ex.Message);
        }
    }

    public async Task<IActionResult> Search(string searchString)
    {
        try
        {
            var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return this.Unauthorized("Invalid token: Missing User ID claim.");
            }

            string userId = userIdClaim.Value;

            var taskDtos = await this._todoListApi.Search(searchString);

            var filteredTasks = taskDtos.Where(t => t.UserId == userId);

            var taskListModels = this._mapper.Map<List<TodoListModel>>(filteredTasks);

            var viewModel = new TodoListWithTasksViewModel
            {
                UserId = userId,
                Tasks = taskListModels,
                SearchString = searchString,
            };

            return this.View("MyTasks", viewModel);
        }
        catch (ApiException ex)
        {
            return this.StatusCode((int)ex.StatusCode, "API Error: " + ex.Message);
        }
    }

    [HttpPost("{todoListId}/{taskId}/change-status")]
    public async Task<IActionResult> ChangeTaskStatus(int todoListId, int taskId, ToDoTaskStatus newStatus)
    {
        try
        {
            var updateStatusRequest = new UpdateTaskStatusRequest { TodoListId = todoListId, NewStatus = newStatus };

            var updatedTaskDto = await this._todoListApi.UpdateTaskStatusAsync(taskId, updateStatusRequest);

            if (updatedTaskDto == null)
            {
                this.TempData["ErrorMessage"] = "Task not found.";
            }
            else
            {
                this.TempData["SuccessMessage"] = "Task status updated successfully!";
            }
        }
        catch (ApiException ex)
        {
            this._logger.LogError(ex, "API error while updating task status: {StatusCode} - {Message}", ex.StatusCode, ex.Message);

            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                this.TempData["ErrorMessage"] = "Task not found.";
            }
            else if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                this.TempData["ErrorMessage"] = "You are not authorized to update this task.";
            }
            else
            {
                this.TempData["ErrorMessage"] = "An error occurred while updating the task status. Please try again later.";
            }
        }

        return this.RedirectToAction("Details", "Task", new { taskId = taskId, todoListId = todoListId });
    }
}
