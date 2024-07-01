using AutoMapper;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Profiles;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        this.CreateMap<TodoListDto, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());
        this.CreateMap<TodoListEntity, TodoListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ToDoTaskStatus.InProgress))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));

        this.CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        this.CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        this.CreateMap<TaskEntity, TodoTaskDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(
                dest => dest.CreatedDate,
                opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));

        this.CreateMap<TodoListDto, TodoListModel>().ReverseMap();
        this.CreateMap<TodoListEntity, TodoListDetailsDto>()
            .ForMember(dest => dest.TaskIds, opt => opt.MapFrom(src => src.Tasks.Select(t => t.Id)));

        this.CreateMap<TaskEntity, TodoTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap()
            .ForMember(dest => dest.TodoList, opt => opt.Ignore())
            .ForMember(dest => dest.TodoListId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

        this.CreateMap<TodoTask, TodoTaskDto>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        this.CreateMap<TagDto, TagEntity>().ReverseMap();
        this.CreateMap<CommentEntity, CommentDto>().ReverseMap();
    }
}
