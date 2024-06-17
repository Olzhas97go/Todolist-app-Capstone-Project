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
        CreateMap<TodoListWebApiModel, TodoListDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
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
