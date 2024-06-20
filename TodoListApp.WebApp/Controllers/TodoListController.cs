using System.Diagnostics;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Refit;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

public class TodoListController : Controller
{
    private readonly ITodoListApi _todoListApi; // Refit interface
    private readonly IMapper _mapper;
    private readonly ILogger<TodoListController> _logger;

    public TodoListController(ITodoListApi todoListApi, IMapper mapper, ILogger<TodoListController> logger)
    {
        _logger = logger;
        _todoListApi = todoListApi;
        _mapper = mapper;
    }

    [HttpGet("todo")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var todoLists = await _todoListApi.GetAllTodoLists();
            var todoListWebApiModel = _mapper.Map<List<TodoListWebApiModel>>(todoLists);
            return View(todoListWebApiModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "No todo lists found.");
                return View(new List<TodoListWebApiModel>()); // Return an empty list
            }
            else
            {
                _logger.LogError(ex, "API error while fetching todo lists.");
                return View("Error",
                    new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Task was canceled while fetching todo lists.");
            return View("Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        catch (JsonSerializationException ex)
        {
            _logger.LogError(ex, "Error deserializing todo list data.");
            return View("Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        catch (AggregateException ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching todo lists: {ErrorMessage}", ex.Message);
            return View("Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View(new TodoListWebApiModel { Tasks = new List<TodoTaskDto> { new TodoTaskDto() } });
    }




    [HttpPost("Create")]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Return to the view with validation errors
        }

        try
        {
            var todoListDto = _mapper.Map<TodoListDto>(model);
            await _todoListApi.CreateTodoList(todoListDto); // Call Refit API method
            return RedirectToAction("Index");
        }
        catch (ApiException apiException)
        {
            ModelState.AddModelError("", apiException.Message);
            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error communicating with the API.");
            ModelState.AddModelError("", "Error communicating with the API. Please try again later.");
            return View(model);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON data from the API.");
            ModelState.AddModelError("", "An error occurred while processing the data. Please try again later.");
            return View(model);
        }
        catch (DbUpdateException ex) // If you expect this in your Web App's service
        {
            _logger.LogError(ex, "Database error while creating todo list.");
            ModelState.AddModelError("", "An error occurred while saving the data. Please try again later.");
            return View(model);
        }
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _todoListApi.DeleteTodoList(id);
            TempData["SuccessMessage"] = "Todo list deleted successfully!"; // Optional success message
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Todo list not found.";
            }
            else
            {
                _logger.LogError(ex, "API error while deleting todo list.");
                TempData["ErrorMessage"] = "An error occurred while deleting the todo list.";
            }
        }

        return RedirectToAction("Index");
    }

    // TodoListController.cs (Web App)

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var todoListDto = await _todoListApi.GetTodoListById(id);

        if (todoListDto == null)
            return NotFound();

        var todoListWebApiModel = _mapper.Map<TodoListWebApiModel>(todoListDto);
        return View(todoListWebApiModel);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var todoListDto = _mapper.Map<TodoListDto>(model);
            await _todoListApi.UpdateTodoList(id, todoListDto);
            return RedirectToAction("Index");
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}
