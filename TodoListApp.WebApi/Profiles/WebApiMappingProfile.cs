using AutoMapper;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Profiles;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        CreateMap<TodoListDto, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());
        CreateMap<TodoListEntity, TodoListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ToDoTaskStatus.InProgress))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));

        CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


        CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<TaskEntity, TodoTaskDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate));


        CreateMap<TodoListDto, TodoListModel>().ReverseMap();
        CreateMap<TodoListEntity, TodoListDetailsDto>()
            .ForMember(dest => dest.TaskIds, opt => opt.MapFrom(src => src.Tasks.Select(t => t.Id)));


        CreateMap<TaskEntity, TodoTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap()
            .ForMember(dest => dest.TodoList, opt => opt.Ignore())
            .ForMember(dest => dest.TodoListId, opt => opt.Ignore());
    }
}
