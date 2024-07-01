using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

[ApiController]
[Route("api/todolist")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService _service;
    private readonly IMapper _mapper;

    public TodoListController(ITodoListDatabaseService service, IMapper mapper)
    {
        this._service = service;
        this._mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAllTodoLists()
    {
        try
        {
            var todoListEntities = await this._service.GetAllTodoListsAsync();
            var todoListModels = this._mapper.Map<List<TodoListModel>>(todoListEntities);
            return this.Ok(todoListModels);
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, "An error occurred while retrieving todo lists.");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Editor")]
    public async Task<ActionResult<TodoListDto>> CreateTodoList([FromBody] TodoListDto todoListDto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState); // Return model validation errors
        }

        try
        {
            var createdTodoList = await this._service.CreateTodoListAsync(todoListDto);

            return this.CreatedAtAction(nameof(this.GetAllTodoLists), new { id = createdTodoList.Id }, createdTodoList); // Return createdTodoList directly
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(new { Error = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            return this.StatusCode(500, new { Error = "An error occurred while creating the todo list." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        try
        {
            var wasDeleted = await this._service.DeleteTodoListAsync(id);
            if (!wasDeleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { Error = "An error occurred while deleting the todo list." });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Owner,Editor")]
    public async Task<IActionResult> UpdateTodoList(int id, [FromBody] TodoListDto todoListDto)
    {
        if (id != todoListDto.Id)
        {
            return this.BadRequest("The id in the URL does not match the id in the request body."); // Ensure IDs match
        }

        try
        {
            var todoListEntity = this._mapper.Map<TodoListEntity>(todoListDto);
            var todoList = await this._service.UpdateTodoListAsync(id, todoListEntity); // Pass both id and entity
            if (todoList == null)
            {
                return this.NotFound();
            }

            return this.Ok(todoList);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await this._service.TodoListExists(id))
            {
                return this.NotFound();
            }
            else
            {
                throw;
            }
        }
        catch (Exception)
        {
            return this.StatusCode(500, "An error occurred while updating the todo list.");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Owner,Editor")]
    public async Task<ActionResult> GetTodoListById(int id)
    {
        var roleClaim = this.User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null)
        {
            var userRole = roleClaim.Value;
        }

        var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return this.Unauthorized("Unauthorized access.");
        }

        var userId = userIdClaim.Value;

        var todoListEntity = await this._service.GetTodoListByIdAsync(id);
        if (todoListEntity == null)
        {
            return this.NotFound();
        }

        var todoListDetailsDto = this._mapper.Map<TodoListDetailsDto>(todoListEntity);
        return this.Ok(todoListDetailsDto);
    }
}
