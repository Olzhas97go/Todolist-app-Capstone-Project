using Microsoft.AspNetCore.Mvc;

public class BackButtonViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackButtonViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        string source = _httpContextAccessor.HttpContext?.Request?.Query["source"];

        if (!string.IsNullOrEmpty(source) && source.ToLower() == "mytasks")
        {
            return View(new BackButtonViewModel
            {
                ActionName = "MyTasks",
                ButtonText = "Back to My Tasks"
            });
        }
        else
        {
            string todoListId = _httpContextAccessor.HttpContext?.Request?.Query["todoListId"];
            if (string.IsNullOrEmpty(todoListId))
            {
                return Content("Error: Missing todoListId."); // Or handle the error differently
            }

            return View(new BackButtonViewModel
            {
                ActionName = "ViewTasks",
                ButtonText = "Back to Tasks",
                TodoListId = todoListId
            });
        }
    }
}

// BackButtonViewModel (no changes)
public class BackButtonViewModel
{
    public string ActionName { get; set; }
    public string ButtonText { get; set; }
    public string TodoListId { get; set; }
}
