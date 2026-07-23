using RobGitHub.Web.Models;

namespace RobGitHub.Web.Services;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> todos =
    [
        new TodoItem { Id = 1, Title = "Sketch out something joyful", CreatedAtUtc = DateTime.UtcNow.AddMinutes(-30) },
        new TodoItem { Id = 2, Title = "Ship the tiny win", CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10) }
    ];

    private readonly Lock syncRoot = new();
    private int nextId = 3;

    public TodoItem Add(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Todo title is required.", nameof(title));
        }

        var normalizedTitle = title.Trim();

        lock (syncRoot)
        {
            var todo = new TodoItem
            {
                Id = nextId++,
                Title = normalizedTitle,
                CreatedAtUtc = DateTime.UtcNow
            };

            todos.Add(todo);
            return todo;
        }
    }

    public IReadOnlyList<TodoItem> GetAll()
    {
        lock (syncRoot)
        {
            return todos
                .OrderBy(todo => todo.IsComplete)
                .ThenByDescending(todo => todo.CreatedAtUtc)
                .ToArray();
        }
    }

    public bool Toggle(int id)
    {
        lock (syncRoot)
        {
            var todo = todos.SingleOrDefault(todo => todo.Id == id);
            if (todo is null)
            {
                return false;
            }

            todo.IsComplete = !todo.IsComplete;
            return true;
        }
    }
}
