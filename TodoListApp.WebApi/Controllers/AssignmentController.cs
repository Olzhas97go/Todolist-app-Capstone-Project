
namespace TodoListApp.WebApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Models.Tasks;


[ApiController]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly TodoListDbContext _context;

    public AssignmentController(TodoListDbContext context)
    {
        _context = context;
    }
}
