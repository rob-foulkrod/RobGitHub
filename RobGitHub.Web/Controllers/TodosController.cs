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
