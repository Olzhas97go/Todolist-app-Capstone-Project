﻿using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Models;

public class TaskDetailsViewModel
{
    public TodoListDto TodoList { get; set; }
    public TodoTask  SelectedTodoTask { get; set; }
    public TodoTask SelectedTask { get; set; }

    public TodoListDetailsDto TodoListDetails { get; set; }

}