namespace TodoListApp.WebApi.Profiles;

using AutoMapper;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        CreateMap<TodoListDto, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore()).ReverseMap();

        CreateMap<TodoListEntity, TodoListModel>();

        CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<TodoListDto, TodoListModel>().ReverseMap();

        CreateMap<TaskEntity, TaskModel>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap();
    }
}
