using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Services;

using AutoMapper;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskModel, TodoListApp.WebApp.Models.Task>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => src.IsCompleted));

        // Map TodoListDto to TodoListWebApiModel
        CreateMap<TodoListDto, TodoListWebApiModel>(); // For mapping data received from the API

        CreateMap<TodoListWebApiModel, TodoListDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<TodoListDto, TodoListModel>();
    }
}
