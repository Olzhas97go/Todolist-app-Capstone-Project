using AutoMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureDbContext(builder.Configuration.GetConnectionString("ToDoListConnection"));

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7171")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.ConfigureServices();
builder.Services.ConfigureAutoMapper();
builder.Services.ConfigureSwagger();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

SeedData.EnsurePopulated(app);
app.UseCors(myAllowSpecificOrigins);
app.Run();
