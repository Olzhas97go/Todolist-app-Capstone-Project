namespace TodoListApp.WebApi.Profiles;

using AutoMapper;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

public class WebApiMappingProfile : Profile
{
    private readonly IMapper _mapper;

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
            .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => _mapper.Map<List<TaskModel>>(src.Tasks)));

        CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


        CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<TodoListDto, TodoListModel>().ReverseMap();

        CreateMap<TaskEntity, TaskModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate < DateTime.Now)).ReverseMap()
            .ForMember(dest => dest.TodoList, opt => opt.Ignore())
            .ForMember(dest => dest.TodoListId, opt => opt.Ignore());
    }
}
