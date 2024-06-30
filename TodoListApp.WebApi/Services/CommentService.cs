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
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all comments");
            var entities = await _context.Comments.ToListAsync();
            return _mapper.Map<IEnumerable<CommentDto>>(entities); // Map using AutoMapper
        }


        public async Task<CommentDto> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting comment by id: {id}");
            var entity = await _context.Comments.FindAsync(id);
            if (entity == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found"); // Throw exception instead of returning null
        }

            return _mapper.Map<CommentDto>(entity); // Use AutoMapper for mapping
        }

        public async Task<CommentDto> CreateAsync(CommentDto dto)
        {
            _logger.LogInformation("Creating a new comment");
            var commentEntity = _mapper.Map<CommentEntity>(dto);
            _context.Comments.Add(commentEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(commentEntity);
        }

        public async Task<CommentDto> UpdateAsync(int id, CommentDto dto)
        {
            _logger.LogInformation($"Updating comment with id: {id}");
            var entity = await _context.Comments.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
        {
            return null;
        }

            _mapper.Map(dto, entity);  // Map updated DTO onto the existing entity
            await _context.SaveChangesAsync();
            return _mapper.Map<CommentDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting comment with id: {id}");
            var entity = await _context.Comments.FindAsync(id);
            if (entity != null)
            {
                _context.Comments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
