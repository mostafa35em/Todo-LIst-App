using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_list_with_mostafa.Data;
using todo_list_with_mostafa.Models;

namespace todo_list_with_mostafa.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public async Task<IActionResult> Index(int? groupId)
        {
            var userId = GetUserId();
            var groups = await _context.TodoGroups
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.CreatedAt)
                .ToListAsync();

            var accounts = await _context.UserAccounts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();

            var currentAccount = accounts.FirstOrDefault(a => a.IsDefault);

            var selectedGroup = groupId.HasValue
                ? groups.FirstOrDefault(g => g.Id == groupId.Value)
                : groups.FirstOrDefault();

            var todoItems = selectedGroup != null
                ? await _context.TodoItems
                    .Include(t => t.Subtasks)
                    .Where(t => t.TodoGroupId == selectedGroup.Id)
                    .OrderBy(t => t.IsCompleted)
                    .ThenByDescending(t => t.CreatedAt)
                    .ToListAsync()
                : new List<TodoItem>();

            var viewModel = new TodoViewModel
            {
                Groups = groups,
                SelectedGroup = selectedGroup,
                TodoItems = todoItems,
                Accounts = accounts,
                CurrentAccount = currentAccount
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(string name, string color)
        {
            var userId = GetUserId();
            var group = new TodoGroup
            {
                Name = name,
                Color = color ?? "#0078D4",
                UserId = userId
            };

            _context.TodoGroups.Add(group);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { groupId = group.Id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(int groupId, string title)
        {
            var todo = new TodoItem
            {
                Title = title,
                TodoGroupId = groupId,
                IsCompleted = false
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTodo(int id, int groupId)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                todo.IsCompleted = !todo.IsCompleted;
                todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTodo(int id, string title, string notes, DateTime? dueDate, int groupId)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                todo.Title = title;
                todo.Notes = notes;
                todo.DueDate = dueDate;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id, int groupId)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubtask(int todoId, string title, int groupId)
        {
            var subtask = new Subtask
            {
                Title = title,
                TodoItemId = todoId,
                IsCompleted = false
            };

            _context.Subtasks.Add(subtask);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubtask(int id, int groupId)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask != null)
            {
                subtask.IsCompleted = !subtask.IsCompleted;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { groupId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(string accountName)
        {
            var userId = GetUserId();
            var account = new UserAccount
            {
                AccountName = accountName,
                IsDefault = false,
                UserId = userId
            };

            _context.UserAccounts.Add(account);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SwitchAccount(int accountId)
        {
            var userId = GetUserId();
            var accounts = await _context.UserAccounts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var account in accounts)
            {
                account.IsDefault = account.Id == accountId;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.TodoGroups.FindAsync(id);
            if (group != null)
            {
                _context.TodoGroups.Remove(group);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}