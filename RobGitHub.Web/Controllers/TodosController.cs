using Microsoft.AspNetCore.Mvc;
using RobGitHub.Web.Models;
using RobGitHub.Web.Services;

namespace RobGitHub.Web.Controllers;

public class TodosController(ITodoRepository todoRepository) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var todos = todoRepository.GetAll();
        return View(new TodoPageViewModel
        {
            Todos = todos,
            PendingCount = todos.Count(todo => !todo.IsComplete)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TodoPageViewModel model)
    {
var titleKey = nameof(TodoPageViewModel.NewTodoTitle);
if (string.IsNullOrWhiteSpace(model.NewTodoTitle) &&
    (!ModelState.TryGetValue(titleKey, out var entry) || entry.Errors.Count == 0))
    ModelState.AddModelError(titleKey, "Todo title is required.");

        if (!ModelState.IsValid)
        {
            var todos = todoRepository.GetAll();
            return View("Index", new TodoPageViewModel
            {
                NewTodoTitle = model.NewTodoTitle,
                Todos = todos,
                PendingCount = todos.Count(todo => !todo.IsComplete)
            });
        }

        todoRepository.Add(model.NewTodoTitle);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Toggle(int id)
    {
        todoRepository.Toggle(id);
        return RedirectToAction(nameof(Index));
    }
}
