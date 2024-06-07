namespace TodoListApp.WebApi.Services;

using AutoMapper;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoList, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore()).ReverseMap();

        CreateMap<TodoListEntity, TodoListModel>();

        CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<TodoList, TodoListModel>().ReverseMap();

        CreateMap<TaskEntity, TaskModel>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap();
    }
}
