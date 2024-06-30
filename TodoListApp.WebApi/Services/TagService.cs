using AutoMapper;
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
    private readonly IMapper _mapper;

    public TagService(TodoListDbContext context, ILogger<TaskService> logger, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all tags");
        var entities = await _context.Tags.ToListAsync();
        return _mapper.Map<List<TagDto>>(entities);
    }

    public async Task<IEnumerable<TodoTaskDto>> GetTasksByTagAsync(string tagText)
    {
        var entities = await _context.Tasks
            .Where(t => t.Tags.Any(tag => tag.Text == tagText))
            .ToListAsync();
        return _mapper.Map<List<TodoTaskDto>>(entities);
    }



    public async Task<TagDto> GetByIdAsync(int id)
    {
        _logger.LogInformation($"Getting tag by id: {id}");
        var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null)
        {
            return null;
        }

        return _mapper.Map<TagDto>(entity);
    }

    public async Task<TagDto> CreateAsync(TagDto dto)
    {
        _logger.LogInformation("Creating a new tag");
        var entity = _mapper.Map<TagEntity>(dto);
        _context.Tags.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TagDto>(entity);
    }

    public async Task<TagDto> UpdateAsync(int id, TagDto dto)
    {
        _logger.LogInformation($"Updating tag with id: {id}");
        var entity = await _context.Tags.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        _mapper.Map(dto, entity);  // Map updated DTO onto the entity
        await _context.SaveChangesAsync();
        return _mapper.Map<TagDto>(entity); // Map back to DTO after saving
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation($"Deleting tag with id: {id}");
        var entity = await _context.Tags.FindAsync(id);
        if (entity != null)
        {
            _context.Tags.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
