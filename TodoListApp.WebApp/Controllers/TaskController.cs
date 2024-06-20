using System.Diagnostics;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refit;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;
using AppTask = TodoListApp.WebApp.Models.Task;
using Task = Microsoft.Build.Utilities.Task;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Models.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var tasks = await _todoListApi.GetTasksForTodoListAsync(todoListId); // Separate call to get tasks

            var viewModel = new TodoListWithTasksViewModel
            {
                TodoList = todoList, Tasks = tasks // No mapping needed
            };

            return View("ViewTasks", viewModel);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching todo list details.");
            return View("Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
}
