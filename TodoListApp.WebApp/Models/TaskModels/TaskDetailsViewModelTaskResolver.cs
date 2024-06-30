using AutoMapper;
using TodoListApp.WebApi.Models.Models;
using Task = TodoListApp.WebApp.Models.TaskModels.Task;

namespace TodoListApp.WebApp.Models;

public class TaskDetailsViewModelTaskResolver : IValueResolver<TodoTaskDto, TaskDetailsViewModel, Task>
{
    public Task Resolve(TodoTaskDto source, TaskDetailsViewModel destination, Task destMember, ResolutionContext context)
    {
        return context.Mapper.Map<Task>(source);
    }
}
