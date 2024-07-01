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
    private readonly IMapper _mapper;
    private readonly TodoListDbContext _context;

    public TagController(ITagService service, TodoListDbContext context, IMapper mapper)
    {
        this._mapper = mapper;
        this._context = context;
        this._service = service;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var result = await this._service.GetAllAsync();
        return this.Ok(result);
    }

    [HttpGet("tasks/{tagText}")]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetTasksByTag(string tagText)
    {
        var result = await this._service.GetTasksByTagAsync(tagText);
        return this.Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var result = await this._service.GetByIdAsync(id);
        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<TagDto>> Create(TagDto dto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        // Check if the task exists
        if (!await this._context.Tasks.AnyAsync(t => t.Id == dto.TaskId))
        {
            return this.BadRequest("Invalid TaskId. The task does not exist.");
        }

        var result = await this._service.CreateAsync(dto);
        return this.CreatedAtAction(nameof(this.GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TagDto>> Update(int id, TagDto dto)
    {
        var result = await this._service.UpdateAsync(id, dto);
        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await this._service.DeleteAsync(id);
        return this.NoContent();
    }

    [HttpGet("task/{taskId}")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsForTask(int taskId)
    {
        try
        {
            var tagDtos = await this._context.Tags.Where(t => t.TaskId == taskId).ToListAsync();
            return this.Ok(this._mapper.Map<IEnumerable<TagDto>>(tagDtos));
        }
        catch (Exception)
        {
            return this.StatusCode(500, "Internal server error");
        }
    }
}
