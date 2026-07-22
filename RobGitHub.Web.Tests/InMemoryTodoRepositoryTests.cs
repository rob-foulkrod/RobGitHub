using RobGitHub.Web.Services;

namespace RobGitHub.Web.Tests;

public class InMemoryTodoRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsPendingItemsBeforeCompletedOnes()
    {
        var repository = new InMemoryTodoRepository();
        repository.Toggle(1);

        var todos = repository.GetAll();

        Assert.False(todos.First().IsComplete);
        Assert.True(todos.Last().IsComplete);
    }

    [Fact]
    public void Add_TrimmedTitle_IsStored()
    {
        var repository = new InMemoryTodoRepository();

        var todo = repository.Add("  Keep the confetti flying  ");

        Assert.Equal("Keep the confetti flying", todo.Title);
        Assert.Contains(repository.GetAll(), item => item.Id == todo.Id);
    }

    [Fact]
    public void Toggle_UnknownId_ReturnsFalse()
    {
        var repository = new InMemoryTodoRepository();

        var changed = repository.Toggle(999);

        Assert.False(changed);
    }
}
