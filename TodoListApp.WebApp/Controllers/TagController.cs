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
    private readonly ITodoListApi _todoListApi; // Refit interface
    private readonly IMapper _mapper;
    private readonly ILogger<TagController> _logger;


    public TagController(ITodoListApi todoListApi, IMapper mapper, ILogger<TagController> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _todoListApi = todoListApi;
    }

    public async Task<IActionResult> TasksByTag(string tagText)
    {
        var tasksByTagDto = await _todoListApi.GetTasksByTagAsync(tagText);

        if (tasksByTagDto.IsSuccessStatusCode && tasksByTagDto.Content != null)
        {
            var tasksByTagViewModel = _mapper.Map<IEnumerable<TodoTaskViewModel>>(tasksByTagDto.Content);

            ViewData["TagText"] = tagText;

            if (!tasksByTagViewModel.Any())
            {
                ViewData["Message"] = $"No tasks found with the tag '{tagText}'.";
                return View("NotFound");
            }

            return View(tasksByTagViewModel);
        }

        // Handle errors
        return View("Error");
    }

    [HttpPost]
    public async Task<IActionResult> AddTag(TagCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Fetch the task details to pass it back to the TaskEdit view
            var task = await _todoListApi.GetTaskByIdAsync(model.TaskId);
            var taskModel = _mapper.Map<TodoListApp.WebApp.Models.TaskModels.Task>(task);

            return View("TaskEdit", taskModel);
        }

        var tagDto = _mapper.Map<TagDto>(model);
        var result = await this._todoListApi.CreateTagAsync(tagDto);

        if (!result.IsSuccessStatusCode)
        {
            // Handle errors, e.g., display an error message or log the error
            // You may also return the TaskEdit view with the task details and show an error message
        }
        else
        {
            TempData["TagAddedSuccessfully"] = true;
        }
        return RedirectToAction("EditTask", "Task", new { taskId = model.TaskId });
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
