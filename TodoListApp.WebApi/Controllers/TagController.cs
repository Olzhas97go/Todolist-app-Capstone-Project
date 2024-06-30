using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;
[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private readonly ITagService _service;
    private readonly ILogger<TodoListController> _logger;
    private readonly IMapper _mapper;
    private readonly TodoListDbContext _context;

    public TagController(ITagService service, ILogger<TodoListController> logger, TodoListDbContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _service = service;
        _logger = logger;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("tasks/{tagText}")]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetTasksByTag(string tagText)
    {
        var result = await _service.GetTasksByTagAsync(tagText);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<TagDto>> Create(TagDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if the task exists
        if (!await _context.Tasks.AnyAsync(t => t.Id == dto.TaskId))
        {
            return BadRequest("Invalid TaskId. The task does not exist.");
        }

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    //
    [HttpPut("{id}")]
    public async Task<ActionResult<TagDto>> Update(int id, TagDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("task/{taskId}")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsForTask(int taskId)
    {
        try
        {
            var tagDtos = await _context.Tags.Where(t => t.TaskId == taskId).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TagDto>>(tagDtos)); // Map to DTOs (if needed)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting tags for task {TaskId}.", taskId);
            return StatusCode(500, "Internal server error");
        }
    }
}
