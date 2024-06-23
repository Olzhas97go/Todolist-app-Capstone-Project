using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Refit;
using TodoListApp.WebApi.Services;
using TodoListApp.WebApp.Areas.Identity.Data;
using TodoListApp.WebApp.Data.TodoListApp;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDb") ?? throw new InvalidOperationException("Connection string 'WebAppContextConnection' not found.");
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
mapperConfig.AssertConfigurationIsValid(); // This will throw an exception if there are any configuration errors.
builder.Services.AddDbContext<WebAppContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<WebAppContext>();


var tokenSigningKey = builder.Configuration["Jwt:TokenSigningKey"];
if (string.IsNullOrEmpty(tokenSigningKey))
{
    throw new InvalidOperationException("JWT TokenSigningKey is not found in the configuration.");
}
var key = Encoding.ASCII.GetBytes(tokenSigningKey);

builder.Services.AddAuthentication()
    .AddCookie(options => {
        options.LoginPath = "/Identity/Account/Login/";
        options.AccessDeniedPath = "/Identity/Account/Login/";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>  // Name the scheme "Bearer"
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"], // Same as issuer for your scenario
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:TokenSigningKey"]))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateTodoListPolicy", policy =>
        policy.RequireAuthenticatedUser() // User must be logged in
            .RequireClaim(ClaimTypes.Role, "Admin")); // User must have the "Admin" role
});
builder.Services.AddDistributedMemoryCache(); // Use in-memory cache for session (simplest option)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Important for security
    options.Cookie.IsEssential = true;
});
 // Replace with your actual Web API base URL
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Update the namespace accordingly
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ITodoListWebApiService, TodoListWebApiService>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<JwtConfiguration>();
builder.Services.AddScoped<IUserManager, UserManagementService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtProvider>();
builder.Services.AddScoped<IApiHeaderService, ApiHeaderService>();
builder.Services.AddTransient<TokenDelegatingHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRefitClient<ITodoListApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:TodoListApiBaseUrl"]);
        c.Timeout = TimeSpan.FromSeconds(10); // Example of a custom timeout
    })
    .AddHttpMessageHandler<TokenDelegatingHandler>();
builder.Services.AddRazorPages();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();


app.UseCors("AllowAnyOrigin");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseMiddleware<JwtTokenMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
