using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;
[ApiController]
[Route("api/todolist/task/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _service;
    private readonly ILogger<TodoListController> _logger;

    public TagController(ITagService service, ILogger<TodoListController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // [HttpGet("{id}")]
    // public async Task<ActionResult<TagDto>> GetById(int id)
    // {
    //     var result = await _service.GetByIdAsync(id);
    //     if (result == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return Ok(result);
    // }
    //
    // [HttpPost]
    // public async Task<ActionResult<TagDto>> Create(TagDto dto)
    // {
    //     var result = await _service.CreateAsync(dto);
    //     return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    // }
    //
    // [HttpPut("{id}")]
    // public async Task<ActionResult<TagDto>> Update(int id, TagDto dto)
    // {
    //     var result = await _service.UpdateAsync(id, dto);
    //     if (result == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return Ok(result);
    // }
    //
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     await _service.DeleteAsync(id);
    //     return NoContent();
    // }
}
