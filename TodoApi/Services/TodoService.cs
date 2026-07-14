using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Todo> CreateTodo(Todo todo)
        {
            if (string.IsNullOrWhiteSpace(todo.Title))
                throw new ArgumentException("Title is required.");

            return await _repository.CreateTodo(todo);
        }

        public async Task<List<Todo>> GetAllTodos()
        {
            return await _repository.GetAllTodos();
        }

        public async Task<Todo?> GetTodoById(int id)
        {
            return await _repository.GetTodoById(id);
        }

        public async Task<Todo?> UpdateTodo(int id, Todo todo)
        {
            var existing = await _repository.GetTodoById(id);

            if (existing == null)
                return null;

            todo.Id = id;

            var updated = await _repository.UpdateTodo(todo);

            return updated ? todo : null;
        }

        public async Task<bool> DeleteTodo(int id)
        {
            return await _repository.DeleteTodo(id);
        }
    }
}