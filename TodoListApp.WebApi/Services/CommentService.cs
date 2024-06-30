using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Services;

    public class CommentService : ICommentService
    {
        private readonly TodoListDbContext _context;
        private readonly ILogger<TaskService> _logger;
        private readonly IMapper _mapper;

        public CommentService(TodoListDbContext context, ILogger<TaskService> logger, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
            this._logger = logger;
        }

        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            var entities = await this._context.Comments.ToListAsync();
            return this._mapper.Map<IEnumerable<CommentDto>>(entities);
        }


        public async Task<CommentDto> GetByIdAsync(int id)
        {
            var entity = await this._context.Comments.FindAsync(id);
            if (entity == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found");
        }

            return this._mapper.Map<CommentDto>(entity);
        }

        public async Task<CommentDto> CreateAsync(CommentDto dto)
        {
            var commentEntity = this._mapper.Map<CommentEntity>(dto);
            this._context.Comments.Add(commentEntity);
            await this._context.SaveChangesAsync();

            return this._mapper.Map<CommentDto>(commentEntity);
        }

        public async Task<CommentDto> UpdateAsync(int id, CommentDto dto)
        {
            var entity = await this._context.Comments.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return null;
            }

            this._mapper.Map(dto, entity);  // Map updated DTO onto the existing entity
            await this._context.SaveChangesAsync();
            return this._mapper.Map<CommentDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this._context.Comments.FindAsync(id);
            if (entity != null)
            {
                this._context.Comments.Remove(entity);
                await this._context.SaveChangesAsync();
            }
        }
    }
