using Microsoft.AspNetCore.Mvc;

namespace TodoListApp.WebApp.Controllers;

public class ErrorController : Controller
{
    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return this.View();
    }
}
