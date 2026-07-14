using Microsoft.Data.Sqlite;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        public TodoRepository()
        {
            InitializeDatabase();
        }
        private readonly string _connectionString = AWSSecretManager.ConnString;
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                );";

                    command.ExecuteNonQuery();
                }
            }
        }
        public async Task<Todo> CreateTodo(Todo todo)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Todos
                        (Title, Description, IsCompleted, CreatedAt)
                        VALUES
                        (@Title, @Description, @Completed, @CreatedAt);

                        SELECT last_insert_rowid();";

                    var createdAt = DateTime.UtcNow;

                    command.Parameters.AddWithValue("@Title", todo.Title ?? string.Empty);
                    command.Parameters.AddWithValue("@Description", (object?)todo.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Completed", todo.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("o"));

                    todo.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    todo.CreatedAt = createdAt;

                    return todo;
                }
            }
        }

        public async Task<List<Todo>> GetAllTodos()
        {
            var todos = new List<Todo>();

            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Todos";

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            todos.Add(new Todo
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                IsCompleted = reader.GetInt32(3) == 1,
                                CreatedAt = DateTime.Parse(reader.GetString(4))
                            });
                        }
                    }
                }
            }

            return todos;
        }

        public async Task<Todo?> GetTodoById(int id)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Todos WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Todo
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                IsCompleted = reader.GetInt32(3) == 1,
                                CreatedAt = DateTime.Parse(reader.GetString(4))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> UpdateTodo(Todo todo)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Todos
                        SET
                            Title = @Title,
                            Description = @Description,
                            IsCompleted = @Completed
                        WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Title", todo.Title ?? string.Empty);
                    command.Parameters.AddWithValue("@Description", (object?)todo.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Completed", todo.IsCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", todo.Id);

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> DeleteTodo(int id)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Todos WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}