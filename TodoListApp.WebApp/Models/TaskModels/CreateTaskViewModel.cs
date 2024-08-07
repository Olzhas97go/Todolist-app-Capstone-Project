﻿using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Models.TaskModels
{
    public class CreateTaskViewModel
    {
        [Required]
        public int TodoListId { get; set; }

        public string TodoListName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.UtcNow;

        public TodoListDetailsDto TodoList { get; set; }

        public string UserId { get; set; } = string.Empty; // Change the type to string

        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}



