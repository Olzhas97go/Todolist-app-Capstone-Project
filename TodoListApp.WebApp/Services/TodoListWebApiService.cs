using System.Security.Claims;
using Newtonsoft.Json;
using TodoListApp.WebApi.Models;
using Microsoft.AspNetCore.Http;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TodoListWebApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;  // Removed the extra underscore here
        _baseUrl = configuration["TodoListApi:BaseUrl"];
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<TodoListModel>> GetTasksForUserAsync()
    {
            var httpContext = _httpContextAccessor.HttpContext; // No need to re-assign

        // Check if the user is authenticated
        if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
        {
            throw new Exception("User not authenticated");
        }

        // Get the user ID from the claims
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            // Handle the case where the user ID claim is missing
            throw new Exception("User ID claim not found.");
        }

        _httpClient.DefaultRequestHeaders.Add("UserId", userId);

        string apiUrl = $"{_baseUrl}/api/TodoList/GetMyTodoLists";
        return await _httpClient.GetFromJsonAsync<List<TodoListModel>>(apiUrl);
    }
}
