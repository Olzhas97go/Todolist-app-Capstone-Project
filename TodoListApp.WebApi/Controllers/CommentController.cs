using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

[ApiController]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        this._service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll()
    {
        var result = await this._service.GetAllAsync();
        return this.Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        var result = await this._service.GetByIdAsync(id);
        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(CommentDto dto)
    {
        var result = await this._service.CreateAsync(dto);
        return this.CreatedAtAction(nameof(this.GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> Update(int id, CommentDto dto)
    {
        var result = await this._service.UpdateAsync(id, dto);
        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await this._service.DeleteAsync(id);
        return this.NoContent();
    }
}
