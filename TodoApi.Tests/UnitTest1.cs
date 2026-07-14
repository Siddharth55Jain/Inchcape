using Xunit;
using TodoApi.Services;
using TodoApi.Repositories;
using TodoApi.Models;

namespace TodoApi.Tests;

public class UnitTest1
{
    private readonly TodoService _service;

    public UnitTest1()
    {
        var repository = new TodoRepository();
        _service = new TodoService(repository);
    }

    [Fact]
    public async Task TestCreateTodo()
    {
        var todo = new Todo
        {
            Title = "Test Todo",
            Description = "Test Description",
            IsCompleted = false
        };

        var result = await _service.CreateTodo(todo);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Todo", result.Title);
    }

    [Fact]
    public async Task TestGetAllTodos()
    {
        var todos = await _service.GetAllTodos();

        Assert.NotNull(todos);
    }

    [Fact]
    public async Task TestGetTodoById()
    {
        var created = await _service.CreateTodo(new Todo
        {
            Title = "Get Todo",
            Description = "Testing"
        });

        var todo = await _service.GetTodoById(created.Id);

        Assert.NotNull(todo);
        Assert.Equal(created.Id, todo.Id);
    }

    [Fact]
    public async Task TestUpdateTodo()
    {
        var created = await _service.CreateTodo(new Todo
        {
            Title = "Old Title",
            Description = "Old Description"
        });

        var updatedTodo = new Todo
        {
            Title = "New Title",
            Description = "New Description",
            IsCompleted = true
        };

        var result = await _service.UpdateTodo(created.Id, updatedTodo);

        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task TestDeleteTodo()
    {
        var created = await _service.CreateTodo(new Todo
        {
            Title = "Delete Me",
            Description = "Delete Test"
        });

        var deleted = await _service.DeleteTodo(created.Id);

        Assert.True(deleted);
    }

    [Fact]
    public async Task GetTodo_InvalidId_ShouldReturnNull()
    {
        var todo = await _service.GetTodoById(-100);

        Assert.Null(todo);
    }

    [Fact]
    public async Task UpdateTodo_InvalidId_ShouldReturnNull()
    {
        var todo = new Todo
        {
            Title = "Invalid",
            Description = "Invalid"
        };

        var result = await _service.UpdateTodo(999999, todo);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTodo_InvalidId_ShouldReturnFalse()
    {
        var result = await _service.DeleteTodo(999999);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateTodo_WithEmptyTitle_ShouldThrowException()
    {
        var todo = new Todo
        {
            Title = "",
            Description = "Description"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateTodo(todo));
    }

    [Fact]
    public async Task CreateTodo_WithNullTitle_ShouldThrowException()
    {
        var todo = new Todo
        {
            Title = null,
            Description = "Description"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateTodo(todo));
    }

    [Fact]
    public async Task TestCompleteCrudFlow()
    {
        var created = await _service.CreateTodo(new Todo
        {
            Title = "CRUD",
            Description = "Flow"
        });

        Assert.True(created.Id > 0);

        var fetched = await _service.GetTodoById(created.Id);

        Assert.NotNull(fetched);

        var updated = await _service.UpdateTodo(created.Id, new Todo
        {
            Title = "Updated CRUD",
            Description = "Updated",
            IsCompleted = true
        });

        Assert.NotNull(updated);

        var deleted = await _service.DeleteTodo(created.Id);

        Assert.True(deleted);

        var deletedTodo = await _service.GetTodoById(created.Id);

        Assert.Null(deletedTodo);
    }
}