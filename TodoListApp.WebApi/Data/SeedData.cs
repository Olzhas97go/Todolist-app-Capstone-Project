using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<TodoListDbContext>();
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }

                if (!context.TodoLists.Any())
                {
                    var todoLists = new List<TodoListEntity>
                    {
                        new TodoListEntity
                        {
                            Name = "Shopping List",
                            Description = "Things to buy at the store",
                            Tasks =
                            {
                                new TaskEntity { Title = "Bread", DueDate = DateTime.Today, Status = ToDoTaskStatus.NotStarted, Description = "Whole wheat bread" },
                                new TaskEntity { Title = "Milk", DueDate = DateTime.Today, Status = ToDoTaskStatus.InProgress, Description = "2% milk" },
                                new TaskEntity { Title = "Eggs", DueDate = DateTime.Today, Status = ToDoTaskStatus.Completed, Description = "A dozen eggs" },
                                new TaskEntity { Title = "Cheese", DueDate = DateTime.Today, Status = ToDoTaskStatus.NotStarted, Description = "Cheddar cheese" },
                            },
                        },
                        new TodoListEntity
                        {
                            Name = "Work Tasks",
                            Tasks =
                            {
                                new TaskEntity { Title = "Finish report", DueDate = DateTime.Today.AddDays(1), Status = ToDoTaskStatus.InProgress, Description = "Monthly sales report" },
                                new TaskEntity { Title = "Schedule meeting with team", DueDate = DateTime.Today.AddDays(2), Status = ToDoTaskStatus.NotStarted, Description = "Discuss project timeline" },
                                new TaskEntity { Title = "Follow up with customer", DueDate = DateTime.Today.AddDays(3), Status = ToDoTaskStatus.Completed, Description = "Follow up on contract negotiation" },
                            },
                        },
                        new TodoListEntity
                        {
                            Name = "Personal Goals",
                            Description = "Things I want to accomplish this year",
                            Tasks =
                            {
                                new TaskEntity { Title = "Run a marathon", DueDate = DateTime.Today.AddMonths(6), Status = ToDoTaskStatus.NotStarted, Description = "Train for the New York marathon" },
                                new TaskEntity { Title = "Learn a new language", DueDate = DateTime.Today.AddMonths(12), Status = ToDoTaskStatus.InProgress, Description = "Start learning Spanish" },
                                new TaskEntity { Title = "Read 50 books", DueDate = DateTime.Today.AddYears(1), Status = ToDoTaskStatus.Completed, Description = "Expand my literary knowledge" },
                            },
                        },
                    };

                    context.TodoLists.AddRange(todoLists);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
