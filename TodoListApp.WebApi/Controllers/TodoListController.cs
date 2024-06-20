namespace TodoListApp.WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApi.Entities;


[ApiController]
[Route("api/todolist")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService  _service;
    private readonly IMapper _mapper;
    private readonly TodoListDbContext _context;
    private readonly ILogger<TodoListController> _logger;

    public TodoListController(ITodoListDatabaseService  service, IMapper mapper, TodoListDbContext context, ILogger<TodoListController> logger)
    {
        _logger = logger;
        _context = context;
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
    public async Task<ActionResult<TodoListDto>> CreateTodoList([FromBody] TodoListDto todoListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Return model validation errors
        }

        try
        {
            _logger.LogInformation("Received request to create todo list: {@todoListDto}", todoListDto);

            var createdTodoList = await _service.CreateTodoListAsync(todoListDto);

            _logger.LogInformation("Returning response with status code 201 (Created): {createdTodoList}", createdTodoList);

            return CreatedAtAction(nameof(GetAllTodoLists), new { id = createdTodoList.Id }, createdTodoList); // Return createdTodoList directly
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid argument when creating todo list: {ErrorMessage}", ex.Message);
            return BadRequest(new { Error = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating todo list");
            return StatusCode(500, new { Error = "An error occurred while creating the todo list." });
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
            return StatusCode(500, new { Error = "An error occurred while deleting the todo list." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoList(int id, [FromBody] TodoListDto todoListDto)
    {
        if (id != todoListDto.Id)
        {
            return BadRequest("The id in the URL does not match the id in the request body."); // Ensure IDs match
        }

        try
        {
            var todoListEntity = _mapper.Map<TodoListEntity>(todoListDto);
            var todoList = await _service.UpdateTodoListAsync(id, todoListEntity); // Pass both id and entity
            if (todoList == null)
            {
                return NotFound(); // Not found
            }
            return Ok(todoList); // Return the updated todo list
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _service.TodoListExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while updating the todo list.");
        }
    }




    [HttpGet("GetMyTodoLists")]
    [Authorize]
    public IActionResult GetMyTodoLists([FromQuery] ToDoTaskStatus? status = null, [FromQuery] string sortBy = "Name", [FromQuery] string sortOrder = "asc")
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("Invalid token: Missing User ID claim.");
        }

        string userId = userIdClaim.Value;
        var tasks = _service.GetTasksForUser(userId, status, sortBy, sortOrder); // Pass sortBy and sortOrder
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTodoListById(int id)
    {
        try
        {
            var todoListEntity = await _service.GetTodoListByIdAsync(id);
            if (todoListEntity == null)
            {
                return NotFound();
            }

            var todoListDetailsDto = _mapper.Map<TodoListDetailsDto>(todoListEntity);
            return Ok(todoListDetailsDto);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError("An error occurred while retrieving the todo list: {ExceptionMessage}\n{ExceptionStackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, new { Error = "An error occurred while retrieving the todo list." });
        }
    }
}
