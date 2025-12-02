using Microsoft.EntityFrameworkCore;
using todo_list_with_mostafa.Models;

namespace todo_list_with_mostafa.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoGroup> TodoGroups { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // TodoGroup configuration
            modelBuilder.Entity<TodoGroup>()
                .HasOne(tg => tg.User)
                .WithMany(u => u.TodoGroups)
                .HasForeignKey(tg => tg.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // TodoItem configuration
            modelBuilder.Entity<TodoItem>()
                .HasOne(ti => ti.TodoGroup)
                .WithMany(tg => tg.TodoItems)
                .HasForeignKey(ti => ti.TodoGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Subtask configuration
            modelBuilder.Entity<Subtask>()
                .HasOne(s => s.TodoItem)
                .WithMany(ti => ti.Subtasks)
                .HasForeignKey(s => s.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAccount configuration
            modelBuilder.Entity<UserAccount>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAccounts)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}