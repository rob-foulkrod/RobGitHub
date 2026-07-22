using System.ComponentModel.DataAnnotations;

namespace RobGitHub.Web.Models;

public class TodoPageViewModel
{
    [Required]
    [StringLength(120)]
    public string NewTodoTitle { get; set; } = string.Empty;

    public IReadOnlyList<TodoItem> Todos { get; init; } = [];

    public int PendingCount { get; init; }
}
