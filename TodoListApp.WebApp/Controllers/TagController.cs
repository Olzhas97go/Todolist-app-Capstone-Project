using AutoMapper;
using Automation;
using Microsoft.AspNetCore.Mvc;
using Refit;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Profiles;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models.TaskModels;

namespace TodoListApp.WebApp.Controllers;

public class TagController : Controller
{
    private readonly ITodoListApi _todoListApi;
    private readonly IMapper _mapper;

    public TagController(ITodoListApi todoListApi, IMapper mapper)
    {
        this._mapper = mapper;
        this._todoListApi = todoListApi;
    }

    public async Task<IActionResult> TasksByTag(string tagText)
    {
        var tasksByTagDto = await this._todoListApi.GetTasksByTagAsync(tagText);

        if (tasksByTagDto.IsSuccessStatusCode && tasksByTagDto.Content != null)
        {
            var tasksByTagViewModel = this._mapper.Map<IEnumerable<TodoTaskViewModel>>(tasksByTagDto.Content);

            this.ViewData["TagText"] = tagText;

            if (!tasksByTagViewModel.Any())
            {
                this.ViewData["Message"] = $"No tasks found with the tag '{tagText}'.";
                return this.View("NotFound");
            }

            return this.View(tasksByTagViewModel);
        }

        return this.View("Error");
    }

    [HttpPost]
    public async Task<IActionResult> AddTag(TagCreateViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            var task = await this._todoListApi.GetTaskByIdAsync(model.TaskId);
            var taskModel = this._mapper.Map<TodoListApp.WebApp.Models.TaskModels.Task>(task);

            return this.View("TaskEdit", taskModel);
        }

        var tagDto = this._mapper.Map<TagDto>(model);
        var result = await this._todoListApi.CreateTagAsync(tagDto);

        if (!result.IsSuccessStatusCode)
        {
        }
        else
        {
            this.TempData["TagAddedSuccessfully"] = true;
        }

        return this.RedirectToAction("EditTask", "Task", new { taskId = model.TaskId });
    }

    // [HttpPost]
    // public async Task<IActionResult> DeleteTag(int tagId, int taskId)
    // {
    //     var response = await _todoListApi.DeleteTagAsync(tagId);
    //
    //     if (response.IsSuccessStatusCode)
    //     {
    //         TempData["TagDeletedSuccessfully"] = true;
    //     }
    //     else
    //     {
    //         TempData["ErrorDeletingTag"] = "There was an error deleting the tag.";
    //     }
    //
    //     return RedirectToAction("EditTask", new { id = taskId });
    // }
}
