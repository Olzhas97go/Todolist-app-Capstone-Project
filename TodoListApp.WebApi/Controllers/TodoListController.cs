namespace TodoListApp.WebApi.Controllers;


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;

[ApiController]
[Route("api/todolist")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService  _service;

    public TodoListController(ITodoListDatabaseService  service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAllTodoLists()
    {
        try
        {
            var todoListEntities = await _service.GetAllTodoListsAsync(); // Get entities
            var todoListModels = todoListEntities.Select(entity => new TodoListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description, // Added Description mapping
            }).ToList(); // Map to TodoListModel

            return Ok(todoListModels); // Return DTOs
        }
        catch (Exception ex)
        {
            // Log the exception (using ILogger if available)
            return StatusCode(500, "An error occurred while retrieving todo lists.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoList>> CreateTodoList([FromBody] TodoList newTodoList)
    {
        try
        {
            var todoList = await this._service.CreateTodoListAsync(newTodoList);
            // Corrected: Use the correct action name and route values
            return CreatedAtAction(nameof(this.GetAllTodoLists), new { id = todoList.Id }, todoList);
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
}
