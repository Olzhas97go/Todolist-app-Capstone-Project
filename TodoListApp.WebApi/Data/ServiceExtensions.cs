namespace TodoListApp.WebApi.Data;

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

public static class ServiceExtensions
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
    }

    public static void ConfigureDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TodoListDbContext>(opts =>
        {
            opts.UseSqlServer(connectionString);
        });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
    }
}
