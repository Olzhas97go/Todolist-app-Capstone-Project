using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;
public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync();
    // Task<TagDto> GetByIdAsync(int id);
    // Task<TagDto> CreateAsync(TagDto dto);
    // Task<TagDto> UpdateAsync(int id, TagDto dto);
    // Task DeleteAsync(int id);
}
