namespace TodoListApp.WebApi.Models.Identity;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models.Tasks;

public class User : IdentityUser, IEquatable<User>
{
    [Key]
    public int Id { get; set; } // Primary key

    [Required]
    public string UserName { get; set; }

    // You might also want to add:
    public string Email { get; set; }

    public string PasswordHash { get; set; } // Or use a better way to store passwords (e.g., hashing with salt)

    public ICollection<TaskEntity> AssignedTasks { get; set; } // Navigation property for assigned tasks

    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    public bool Equals(User? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }
}
