using AutoMapper;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Models;

public class TaskDetailsViewModelTaskResolver : IValueResolver<TodoTaskDto, TaskDetailsViewModel, TodoListApp.WebApp.Models.Task>
{
    public TodoListApp.WebApp.Models.Task Resolve(TodoTaskDto source, TaskDetailsViewModel destination, TodoListApp.WebApp.Models.Task destMember, ResolutionContext context)
    {
        // Map the Task property using the existing mapping configuration
        return context.Mapper.Map<TodoListApp.WebApp.Models.Task>(source);
    }
}
