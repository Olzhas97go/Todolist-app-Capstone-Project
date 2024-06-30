using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllAsync();

    Task<CommentDto> GetByIdAsync(int id);

    Task<CommentDto> CreateAsync(CommentDto dto);

    Task<CommentDto> UpdateAsync(int id, CommentDto dto);

    Task DeleteAsync(int id);
}
