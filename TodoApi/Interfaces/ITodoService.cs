using TodoApi.Models;

namespace TodoApi.Interfaces
{
    public interface ITodoService
    {
        Task<Todo> CreateTodo(Todo todo);
        Task<List<Todo>> GetAllTodos();
        Task<Todo?> GetTodoById(int id);
        Task<Todo?> UpdateTodo(int id, Todo todo);
        Task<bool> DeleteTodo(int id);
    }
}
