using AutoMapper;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Models.TaskModels;
using Task = TodoListApp.WebApp.Models.TaskModels.Task;

namespace TodoListApp.WebApp.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        this.CreateMap<TodoListApp.WebApi.Models.Models.TodoTask, Task>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());

        this.CreateMap<TodoTaskDto, TodoTask>()
            .ForMember(dest => dest.TodoListId, opt => opt.MapFrom(src => src.TodoListId))
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        this.CreateMap<Task, TodoTaskDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.CompletedAt > src.CreatedDate));

        this.CreateMap<TodoListDetailsDto, TodoListWebApiModel>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap();

        this.CreateMap<TodoListWebApiModel, TodoListDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        this.CreateMap<TaskEntity, TodoTask>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

        this.CreateMap<CreateTaskViewModel, TodoTaskDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore());

        this.CreateMap<TodoListDto, TodoListWebApiModel>();
        this.CreateMap<TaskEntity, TodoTaskDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ToDoTaskStatus)src.Status));

        this.CreateMap<TaskEntity, Task>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.IsCompleted ? DateTime.UtcNow : (DateTime?)null))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ReverseMap();
        this.CreateMap<TodoListModel, TodoTask>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.DueDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.TodoListId, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore());

        this.CreateMap<TodoTask, TodoListModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        this.CreateMap<TodoTask, TodoTaskDto>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));

        this.CreateMap<TodoListDetailsDto, TodoListDto>()
            .ForMember(
                dest => dest.Tasks,
                opt => opt.MapFrom(src => src.TaskIds.Select(id => new TodoTaskDto { Id = id })))
            .ForMember(dest => dest.Status, opt => opt.Ignore());
        this.CreateMap<TodoTaskDto, TodoTaskViewModel>()
            .ReverseMap();
        this.CreateMap<TagCreateViewModel, TagDto>()
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());
    }
}
