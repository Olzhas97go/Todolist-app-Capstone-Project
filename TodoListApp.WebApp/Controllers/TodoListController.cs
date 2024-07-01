using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Refit;
using TodoListApp.WebApi.Models.Models;
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
        this._logger = logger;
        this._todoListApi = todoListApi;
        this._mapper = mapper;
    }

    [HttpGet("todo")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var todoLists = await this._todoListApi.GetAllTodoLists();
            var todoListWebApiModel = this._mapper.Map<List<TodoListWebApiModel>>(todoLists);
            return this.View(todoListWebApiModel);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                this._logger.LogWarning(ex, "No todo lists found.");
                return this.View(new List<TodoListWebApiModel>()); // Return an empty list
            }
            else
            {
                this._logger.LogError(ex, "API error while fetching todo lists.");
                return this.View(
                    "Error",
                    new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
            }
        }
        catch (TaskCanceledException ex)
        {
            this._logger.LogError(ex, "Task was canceled while fetching todo lists.");
            return this.View(
                "Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
        catch (JsonSerializationException ex)
        {
            this._logger.LogError(ex, "Error deserializing todo list data.");
            return this.View(
                "Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
        catch (AggregateException ex)
        {
            this._logger.LogError(ex, "An unexpected error occurred while fetching todo lists: {ErrorMessage}", ex.Message);
            return this.View(
                "Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (!this.User.Identity.IsAuthenticated)
        {
            return this.Redirect("/Identity/Account/Login?ReturnUrl=%2F");
        }

        var model = new TodoListWebApiModel
        {
            Tasks = new List<TodoTaskDto> { new TodoTaskDto() },
        };

        return this.View(model);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
        foreach (var task in model.Tasks)
        {
            task.UserId = userId;
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            var todoListDto = this._mapper.Map<TodoListDto>(model);
            await this._todoListApi.CreateTodoList(todoListDto);
            return this.RedirectToAction("Index");
        }
        catch (ApiException apiException)
        {
            if (apiException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return this.Redirect("/Identity/Account/Login?ReturnUrl=%2F");
            }

            this.ModelState.AddModelError(" ", apiException.Message);
            return this.View(model);
        }
        catch (HttpRequestException ex)
        {
            this._logger.LogError(ex, "Error communicating with the API.");
            this.ModelState.AddModelError(" ", "Error communicating with the API. Please try again later.");
            return this.View(model);
        }
        catch (JsonException ex)
        {
            this._logger.LogError(ex, "Error parsing JSON data from the API.");
            this.ModelState.AddModelError(" ", "An error occurred while processing the data. Please try again later.");
            return this.View(model);
        }
        catch (DbUpdateException ex)
        {
            this._logger.LogError(ex, "Database error while creating todo list.");
            this.ModelState.AddModelError(" ", "An error occurred while saving the data. Please try again later.");
            return this.View(model);
        }
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await this._todoListApi.DeleteTodoList(id);
            this.TempData["SuccessMessage"] = "Todo list deleted successfully!";
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                this.TempData["ErrorMessage"] = "You do not have permission to delete this todo list.";
            }
            else if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                this.TempData["ErrorMessage"] = "Todo list not found.";
            }
            else
            {
                this._logger.LogError(ex, "API error while deleting todo list.");
                this.TempData["ErrorMessage"] = "An error occurred while deleting the todo list.";
            }
        }

        return this.RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var todoListDto = await this._todoListApi.GetTodoListById(id);

        if (todoListDto == null)
        {
            return this.NotFound();
        }

        var todoListWebApiModel = this._mapper.Map<TodoListWebApiModel>(todoListDto);
        return this.View(todoListWebApiModel);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        var roleClaim = this.User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null)
        {
            var userRole = roleClaim.Value;
            this._logger.LogInformation("User role: {UserRole}", userRole);
        }
        else
        {
            this._logger.LogWarning("User role claim not found");
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            var todoListDto = this._mapper.Map<TodoListDto>(model);
            await this._todoListApi.UpdateTodoList(id, todoListDto);
            return this.RedirectToAction("Index");
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                this._logger.LogWarning("Forbidden access while updating to-do list (ID: {TodoListId})", id);
                this.TempData["ErrorMessage"] = "You do not have permission to edit this to-do list.";
                return this.RedirectToAction("AccessDenied", "Error");
            }

            this._logger.LogError(ex, "An error occurred while updating the to-do list (ID: {TodoListId})", id);
            this.ModelState.AddModelError(" ", "An error occurred while updating the to-do list. Please try again later.");
            return this.View(model);
        }
    }
}
