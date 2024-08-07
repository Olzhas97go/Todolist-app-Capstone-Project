﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Profiles;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Data;

public static class ServiceExtensions
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenSigningKey = configuration["Jwt:TokenSigningKey"];
        if (string.IsNullOrEmpty(tokenSigningKey))
        {
            throw new InvalidOperationException("JWT TokenSigningKey is not configured.");
        }

        var issuerSigningKey = Encoding.UTF8.GetBytes(tokenSigningKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(issuerSigningKey),
                    ValidateIssuer = configuration.GetValue<bool>("Jwt:ValidateIssuer", false),
                    ValidateAudience = configuration.GetValue<bool>("Jwt:ValidateAudience", false),
                    ValidateLifetime = true,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["jwtToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OwnerPolicy", policy => policy.RequireRole(UserRoles.Owner.ToString()));
            options.AddPolicy("EditorPolicy", policy => policy.RequireRole(UserRoles.Editor.ToString()));
            options.AddPolicy("ViewerPolicy", policy => policy.RequireRole(UserRoles.Viewer.ToString()));
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
                Type = SecuritySchemeType.ApiKey,
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
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ICommentService, CommentService>();
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(WebApiMappingProfile).Assembly);
    }
}
