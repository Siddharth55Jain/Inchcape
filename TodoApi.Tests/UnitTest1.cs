using TodoApi.Interfaces;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests
{
    public class UnitTest1
    {
        private readonly TodoService _service;

        public UnitTest1()
        {
            _service = new TodoService(new FakeTodoRepository());
        }

        [Fact]
        public async Task CreateTodo_Test()
        {
            var todo = new Todo
            {
                Title = "Test",
                Description = "Description",
                IsCompleted = false
            };

            var result = await _service.CreateTodo(todo);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task GetTodoById_Test()
        {
            var result = await _service.GetTodoById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAllTodos_Test()
        {
            var result = await _service.GetAllTodos();

            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task UpdateTodo_Test()
        {
            var todo = new Todo
            {
                Title = "Updated",
                Description = "Updated Description",
                IsCompleted = true
            };

            var result = await _service.UpdateTodo(1, todo);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
        }

        [Fact]
        public async Task DeleteTodo_Test()
        {
            var result = await _service.DeleteTodo(1);

            Assert.True(result);
        }
    }

    internal class FakeTodoRepository : ITodoRepository
    {
        private readonly List<Todo> _todos = new()
        {
            new Todo
            {
                Id = 1,
                Title = "Sample",
                Description = "Sample Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        public Task<Todo> CreateTodo(Todo todo)
        {
            todo.Id = _todos.Count + 1;
            todo.CreatedAt = DateTime.UtcNow;
            _todos.Add(todo);

            return Task.FromResult(todo);
        }

        public Task<List<Todo>> GetAllTodos()
        {
            return Task.FromResult(_todos);
        }

        public Task<Todo?> GetTodoById(int id)
        {
            return Task.FromResult(_todos.FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> UpdateTodo(Todo todo)
        {
            var existing = _todos.FirstOrDefault(x => x.Id == todo.Id);

            if (existing == null)
                return Task.FromResult(false);

            existing.Title = todo.Title;
            existing.Description = todo.Description;
            existing.IsCompleted = todo.IsCompleted;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteTodo(int id)
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);

            if (todo == null)
                return Task.FromResult(false);

            _todos.Remove(todo);

            return Task.FromResult(true);
        }
    }
}