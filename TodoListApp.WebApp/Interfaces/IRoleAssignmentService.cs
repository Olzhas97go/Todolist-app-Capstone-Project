namespace TodoListApp.WebApp.Interfaces;

public interface IRoleAssignmentService
{
    Task<string> DetermineRoleAsync(string email);
}
