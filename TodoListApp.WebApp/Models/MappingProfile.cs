using AutoMapper;
using Automation;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Profiles;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.TaskModels;
using Task = TodoListApp.WebApp.Models.TaskModels.Task;

namespace TodoListApp.WebApi.Services;



public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoListApp.WebApi.Models.Models.TodoTask, Task>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());

        CreateMap<TodoTaskDto, TodoTask>()
            .ForMember(dest => dest.TodoListId, opt => opt.MapFrom(src => src.TodoListId))
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.UserId, opt => opt.Ignore());


        CreateMap<Task, TodoTaskDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))// Cast to int
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.CompletedAt > src.CreatedDate));

        // Map TodoListDto to TodoListWebApiModel
        CreateMap<TodoListDetailsDto, TodoListWebApiModel>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap();

        CreateMap<TodoListWebApiModel, TodoListDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<TaskEntity, TodoTask>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

        CreateMap<CreateTaskViewModel, TodoTaskDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore());

        CreateMap<TodoListDto, TodoListWebApiModel>();
        CreateMap<TaskEntity, TodoTaskDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ToDoTaskStatus)src.Status)); // Cast to enum


        CreateMap<TaskEntity, Task>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.IsCompleted ? DateTime.UtcNow : (DateTime?)null))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ReverseMap();
        CreateMap<TodoListModel, TodoTask>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Or provide a default value
            .ForMember(dest => dest.DueDate, opt => opt.Ignore()) // Or provide a default value
            .ForMember(dest => dest.IsCompleted, opt => opt.Ignore()) // Or provide a default value
            .ForMember(dest => dest.TodoListId, opt => opt.Ignore()) // Or provide a default value
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore()); // Or provide a default value

        CreateMap<TodoTask, TodoListModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)); // Add this line

        CreateMap<TodoTask, TodoTaskDto>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));

        // In your MappingProfile class
        CreateMap<TodoListDetailsDto, TodoListDto>()
            .ForMember(dest => dest.Tasks, // Assuming TodoListDto has a Tasks property
                opt => opt.MapFrom(src => src.TaskIds.Select(id => new TodoTaskDto { Id = id })))
            .ForMember(dest => dest.Status, opt => opt.Ignore());
        CreateMap<TodoTaskDto, TodoTaskViewModel>()
            .ReverseMap();
        CreateMap<TagCreateViewModel, TagDto>()
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());
    }
}
