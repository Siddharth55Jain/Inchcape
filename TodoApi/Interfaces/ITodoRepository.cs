using TodoApi.Models;

namespace TodoApi.Interfaces
{
    public interface ITodoRepository
    {
        Task<Todo> CreateTodo(Todo todo);
        Task<List<Todo>> GetAllTodos();
        Task<Todo?> GetTodoById(int id);
        Task<bool> UpdateTodo(Todo todo);
        Task<bool> DeleteTodo(int id);
    }
}