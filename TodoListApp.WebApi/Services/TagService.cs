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
    private readonly IMapper _mapper;

    public TagService(TodoListDbContext context, IMapper mapper)
    {
        this._mapper = mapper;
        this._context = context;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        var entities = await this._context.Tags.ToListAsync();
        return this._mapper.Map<List<TagDto>>(entities);
    }

    public async Task<IEnumerable<TodoTaskDto>> GetTasksByTagAsync(string tagText)
    {
        var entities = await this._context.Tasks
            .Where(t => t.Tags.Any(tag => tag.Text == tagText))
            .ToListAsync();
        return this._mapper.Map<List<TodoTaskDto>>(entities);
    }

    public async Task<TagDto> GetByIdAsync(int id)
    {
        var entity = await this._context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null)
        {
            return null;
        }

        return this._mapper.Map<TagDto>(entity);
    }

    public async Task<TagDto> CreateAsync(TagDto dto)
    {
        var entity = this._mapper.Map<TagEntity>(dto);
        this._context.Tags.Add(entity);
        await this._context.SaveChangesAsync();
        return this._mapper.Map<TagDto>(entity);
    }

    public async Task<TagDto> UpdateAsync(int id, TagDto dto)
    {
        var entity = await this._context.Tags.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        this._mapper.Map(dto, entity);
        await this._context.SaveChangesAsync();
        return this._mapper.Map<TagDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this._context.Tags.FindAsync(id);
        if (entity != null)
        {
            this._context.Tags.Remove(entity);
            await this._context.SaveChangesAsync();
        }
    }
}
