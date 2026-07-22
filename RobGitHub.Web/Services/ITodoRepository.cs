using RobGitHub.Web.Models;

namespace RobGitHub.Web.Services;

public interface ITodoRepository
{
    IReadOnlyList<TodoItem> GetAll();

    TodoItem Add(string title);

    bool Toggle(int id);
}
