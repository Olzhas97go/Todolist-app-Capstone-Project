using System.Security.Claims;
using Newtonsoft.Json;
using TodoListApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TodoListWebApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        // Inject ApiSettings to access the base URL from configuration.
        _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<TodoListModel>> GetTasksForUserAsync()
    {
        // No need to manually add the token; the cookie middleware will handle it.
        var apiUrl = $"{_apiSettings.TodoListApiBaseUrl}/api/TodoListDto/GetMyTodoLists";
        return await _httpClient.GetFromJsonAsync<List<TodoListModel>>(apiUrl);
    }

    public async Task<List<TodoListDto>> GetTodoListsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiSettings.TodoListApiBaseUrl}/api/TodoList");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TodoListDto>>();
    }

    public async Task<TodoListDto> GetTodoListByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_apiSettings.TodoListApiBaseUrl}/api/TodoList/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoListDto>();
    }

    public async Task<TodoListDto> CreateTodoListAsync(TodoListDto todoListDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiSettings.TodoListApiBaseUrl}/api/TodoList", todoListDto);
        response.EnsureSuccessStatusCode(); // Ensure the request was successful

        // Read the response content as TodoListDto
        var createdTodoList = await response.Content.ReadFromJsonAsync<TodoListDto>();

        return createdTodoList; // Return the created TodoListDto
    }


    public async Task UpdateTaskStatusAsync(int taskId, ToDoTaskStatus newStatus)
    {
        var apiUrl = $"{_apiSettings.TodoListApiBaseUrl}/api/TodoListDto/tasks/{taskId}/status";

        var content = JsonContent.Create(new { Status = newStatus });
        var response = await _httpClient.PutAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
    }
}
