using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class BackButtonViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackButtonViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        var referer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
        var todoListId = _httpContextAccessor.HttpContext.Request.Query["todoListId"];

        return View(new BackButtonViewModel
        {
            ActionName = referer.Contains("/MyTasks") ? "MyTasks" : "ViewTasks",
            ButtonText = referer.Contains("/MyTasks") ? "Back to My Tasks" : "Back to Tasks",
            TodoListId = todoListId
        });
    }
}


public class BackButtonViewModel
{
    public string ActionName { get; set; }
    public string ButtonText { get; set; }
    public string TodoListId { get; set; }
}
