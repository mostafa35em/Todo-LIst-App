using System.ComponentModel.DataAnnotations;

namespace todo_list_with_mostafa.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TodoGroup> TodoGroups { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }

    public class TodoGroup
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Color { get; set; } = "#0078D4";

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }

    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Notes { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ReminderDate { get; set; }

        public int TodoGroupId { get; set; }
        public virtual TodoGroup TodoGroup { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public virtual ICollection<Subtask> Subtasks { get; set; }
    }

    public class Subtask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public int TodoItemId { get; set; }
        public virtual TodoItem TodoItem { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        public string AccountName { get; set; }

        public bool IsDefault { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // View Models
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class TodoViewModel
    {
        public List<TodoGroup> Groups { get; set; }
        public TodoGroup SelectedGroup { get; set; }
        public List<TodoItem> TodoItems { get; set; }
        public List<UserAccount> Accounts { get; set; }
        public UserAccount CurrentAccount { get; set; }
    }
}