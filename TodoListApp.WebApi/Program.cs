using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Identity;
using TodoListApp.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TodoListDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ToDoListConnection");

    // Log the connection string to the console
    Console.WriteLine(connectionString);

    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddTransient<UserSeeder>();
builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
configuration.AssertConfigurationIsValid();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

using var scope = app.Services.CreateScope(); // Create a scope to resolve dependencies
var seeder = scope.ServiceProvider.GetRequiredService<UserSeeder>();
await seeder.SeedAsync();

app.MapControllers();

SeedData.EnsurePopulated(app);

app.Run();
