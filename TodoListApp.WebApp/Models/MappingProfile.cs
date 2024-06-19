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
        CreateMap<TodoListDto, TodoListWebApiModel>();

        // In WebApiMappingProfile.cs
        // In your WebApiMappingProfile.cs file
        CreateMap<TodoListApp.WebApi.Models.Tasks.TaskEntity, TodoTaskDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));


        CreateMap<TodoListWebApiModel, TodoListDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignore UserId
            .ForMember(dest => dest.Status, opt => opt.Ignore());  // Ignore Status

        CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
        CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());
        CreateMap<TodoListDto, TodoListModel>();
        CreateMap<TaskEntity, TaskModel>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap();
    }
}
