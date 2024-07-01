using Microsoft.AspNetCore.Mvc;

public class BackButtonViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackButtonViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        string source = this._httpContextAccessor.HttpContext?.Request?.Query["source"];

        if (!string.IsNullOrEmpty(source) && source.ToLower() == "mytasks")
        {
            return this.View(new BackButtonViewModel
            {
                ActionName = "MyTasks",
                ButtonText = "Back to My Tasks",
            });
        }
        else
        {
            string todoListId = _httpContextAccessor.HttpContext?.Request?.Query["todoListId"];
            if (string.IsNullOrEmpty(todoListId))
            {
                return this.Content("Error: Missing todoListId."); // Or handle the error differently
            }

            return this.View(new BackButtonViewModel
            {
                ActionName = "ViewTasks",
                ButtonText = "Back to Tasks",
                TodoListId = todoListId,
            });
        }
    }
}

public class BackButtonViewModel
{
    public string ActionName { get; set; }

    public string ButtonText { get; set; }

    public string TodoListId { get; set; }
}
