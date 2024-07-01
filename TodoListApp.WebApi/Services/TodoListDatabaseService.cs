using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext _context;
    private readonly IMapper _mapper;

    public TodoListDatabaseService(TodoListDbContext context, IMapper mapper)
    {
        this._context = context;
        this._mapper = mapper;
    }

    public async Task<bool> TodoListExists(int id)
    {
        return await this._context.TodoLists.AnyAsync(e => e.Id == id);
    }

    public async Task<List<TodoListModel>> GetAllTodoListsAsync()
    {
        try
        {
            var entities = await this._context.TodoLists.ToListAsync();
            return this._mapper.Map<List<TodoListModel>>(entities);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving todo lists.", ex);
        }
    }

    public async Task<TodoListDto> CreateTodoListAsync(TodoListDto newTodoListDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newTodoListDto.Name))
            {
                throw new ArgumentException("Todo list name cannot be empty.");
            }

            var todoListEntity = this._mapper.Map<TodoListEntity>(newTodoListDto);

            todoListEntity.Id = 0;

            this._context.TodoLists.Add(todoListEntity);
            await this._context.SaveChangesAsync();

            return this._mapper.Map<TodoListDto>(todoListEntity);
        }
        catch (ArgumentException ex)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while saving the data.", ex);
        }
    }

    public async Task<bool> DeleteTodoListAsync(int id)
    {
        try
        {
            var todoList = await this._context.TodoLists.FindAsync(id);
            if (todoList == null)
            {
                return false;
            }

            this._context.TodoLists.Remove(todoList);
            await this._context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw;
        }
    }

    public async Task<TodoListEntity> UpdateTodoListAsync(int id, TodoListEntity todoList)
    {
        var existingTodoList = await this._context.TodoLists.FindAsync(id);
        if (existingTodoList == null)
        {
            return null;
        }

        existingTodoList.Name = todoList.Name;
        existingTodoList.Description = todoList.Description;

        try
        {
            await this._context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency exception here if needed
            throw;
        }

        return existingTodoList;
    }

    public async Task<TodoListEntity> GetTodoListByIdAsync(int id)
    {
        var todoListEntity = await this._context.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => tl.Id == id);

        if (todoListEntity == null)
        {
            return null;
        }

        return todoListEntity;
    }
}
