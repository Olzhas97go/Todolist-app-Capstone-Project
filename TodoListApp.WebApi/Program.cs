using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.DataContext;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TodoListDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ToDoListConnection"));
});

builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.MapControllers();

app.Run();
