using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Services;
public class TagService : ITagService
{
    private readonly TodoListDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TagService(TodoListDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all tags");
        var entities = await _context.Tags.ToListAsync();
        return entities.Adapt<List<TagDto>>();
    }

    // public async Task<TagDto> GetByIdAsync(int id)
    // {
    //     _logger.LogInformation($"Getting tag by id: {id}");
    //     var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
    //     if (entity == null)
    //     {
    //         return null;
    //     }
    //
    //     //return entity.Adapt<TagDto>();
    // }

    // public async Task<TagDto> CreateAsync(TagDto dto)
    // {
    //     _logger.LogInformation("Creating a new tag");
    //     var entity = dto.Adapt<TagEntity>();
    //     _context.Tags.Add(entity);
    //     await _context.SaveChangesAsync();
    //     return entity.Adapt<TagDto>();
    // }

    // public async Task<TagDto> UpdateAsync(int id, TagDto dto)
    // {
    //     _logger.LogInformation($"Updating tag with id: {id}");
    //     var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
    //     if (entity == null) return null;
    //
    //     dto.Adapt(entity);
    //     await _context.SaveChangesAsync();
    //     return entity.Adapt<TagDto>();
    // }

    // public async Task DeleteAsync(int id)
    // {
    //     _logger.LogInformation($"Deleting tag with id: {id}");
    //     var entity = await _context.Tags.FindAsync(id);
    //     if (entity != null)
    //     {
    //         _context.Tags.Remove(entity);
    //         await _context.SaveChangesAsync();
    //     }
    // }
}
