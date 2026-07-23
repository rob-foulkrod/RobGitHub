using Microsoft.AspNetCore.Mvc;
using RobGitHub.Web.Controllers;
using RobGitHub.Web.Models;
using RobGitHub.Web.Services;

namespace RobGitHub.Web.Tests;

public class TodosControllerTests
{
    [Fact]
    public void Index_ReturnsViewModelWithPendingCount()
    {
        var repository = new InMemoryTodoRepository();
        repository.Toggle(2);
        var controller = new TodosController(repository);

        var result = controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TodoPageViewModel>(viewResult.Model);
        Assert.Equal(1, model.PendingCount);
        Assert.Equal(2, model.Todos.Count);
    }

    [Fact]
    public void Create_WithValidModel_AddsTrimmedTodoAndRedirects()
    {
        var repository = new InMemoryTodoRepository();
        var controller = new TodosController(repository);

        var result = controller.Create(new TodoPageViewModel { NewTodoTitle = "  Celebrate tiny shipping wins  " });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(TodosController.Index), redirect.ActionName);
        Assert.Contains(repository.GetAll(), todo => todo.Title == "Celebrate tiny shipping wins");
    }

    [Fact]
    public void Create_WithInvalidModel_ReturnsIndexView()
    {
        var repository = new InMemoryTodoRepository();
        var controller = new TodosController(repository);
        controller.ModelState.AddModelError(nameof(TodoPageViewModel.NewTodoTitle), "Required");

        var result = controller.Create(new TodoPageViewModel());

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
    }

    [Fact]
    public void Create_WithWhitespaceOnlyTitle_ReturnsIndexViewAndDoesNotAddTodo()
    {
        var repository = new InMemoryTodoRepository();
        var initialCount = repository.GetAll().Count;
        var controller = new TodosController(repository);

        var result = controller.Create(new TodoPageViewModel { NewTodoTitle = "   " });

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(initialCount, repository.GetAll().Count);
        Assert.DoesNotContain(repository.GetAll(), todo => string.IsNullOrWhiteSpace(todo.Title));
    }
}
