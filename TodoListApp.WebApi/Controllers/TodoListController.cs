namespace TodoListApp.WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/todolist")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService  _service;
    private readonly IMapper _mapper;

    public TodoListController(ITodoListDatabaseService  service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAllTodoLists()
    {
        try
        {
            var todoListEntities = await _service.GetAllTodoListsAsync();
            var todoListModels = _mapper.Map<List<TodoListModel>>(todoListEntities);
            return Ok(todoListModels);
        }
        catch (Exception ex)
        {
            // Логирование исключения
            return StatusCode(500, "An error occurred while retrieving todo lists.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoList>> CreateTodoList([FromBody] TodoList newTodoList)
    {
        try
        {
            var createdTodoList = await _service.CreateTodoListAsync(newTodoList); // Ensure newTodoList does not have an Id set
            var todoListModel = _mapper.Map<TodoListModel>(createdTodoList);
            return CreatedAtAction(nameof(GetAllTodoLists), new { id = todoListModel.Id }, todoListModel);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400 Bad Request
        }
        catch (DbUpdateException ex)
        {
            // Log the exception (using ILogger if available)
            return StatusCode(500, "An error occurred while creating the todo list."); // 500 Internal Server Error
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        try
        {
            var wasDeleted = await this._service.DeleteTodoListAsync(id);
            if (!wasDeleted)
            {
                return NotFound(); // Return 404 if the todo list doesn't exist
            }
            return NoContent(); // Return 204 No Content on successful deletion
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while deleting the todo list.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoList(int id, [FromBody] TodoList updatedTodoList)
    {
        if (id != updatedTodoList.Id)
        {
            return BadRequest("The id in the URL does not match the id in the request body."); // Ensure IDs match
        }

        try
        {
            var todoList = await this._service.UpdateTodoListAsync(id, updatedTodoList);
            if (todoList == null)
            {
                return NotFound(); // Not found
            }
            return Ok(todoList); // Return the updated todo list
        }
        catch (DbUpdateConcurrencyException)
        {
            // You could try to handle the concurrency conflict here or simply return an error
            return Conflict(); // 409 Conflict
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while updating the todo list.");
        }
    }

    [HttpGet("GetMyTodoLists")]
    [Authorize]
    public IActionResult GetMyTodoLists()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("Invalid token: Missing User ID claim.");
        }

        string userId = userIdClaim.Value;
        var tasks = _service.GetTasksForUser(userId);
        return Ok(tasks);
    }
}
