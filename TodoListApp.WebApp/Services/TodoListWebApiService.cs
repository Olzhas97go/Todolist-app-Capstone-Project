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
        // No need to manually add the token; the cookie middleware will handle it.
        string apiUrl = $"{_baseUrl}/api/TodoList/GetMyTodoLists";
        return await _httpClient.GetFromJsonAsync<List<TodoListModel>>(apiUrl);
    }

    public async Task UpdateTaskStatusAsync(int taskId, ToDoTaskStatus newStatus)
    {
        var apiUrl = $"{_baseUrl}/api/TodoList/tasks/{taskId}/status";

        var content = JsonContent.Create(new { Status = newStatus }); // Serialize the newStatus
        var response = await _httpClient.PutAsync(apiUrl, content);
        response.EnsureSuccessStatusCode(); // Throw exception if not successful
    }
}
