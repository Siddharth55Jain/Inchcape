using Microsoft.AspNetCore.Mvc;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(
            ITodoService todoService,
            ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
        {
            try
            {
                var result = await _todoService.CreateTodo(todo);
                if (todo == null)
                    return BadRequest("Unable to create todo");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating todo.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoById(id);

                if (todo == null)
                    return NotFound($"Todo with Id {id} not found.");

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting todo.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodo()
        {
            try
            {
                var todos = await _todoService.GetAllTodos();

                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting todos.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(
            int id,
            [FromBody] Todo todo)
        {
            try
            {
                var updatedTodo = await _todoService.UpdateTodo(id, todo);

                if (updatedTodo == null)
                    return NotFound($"Todo with Id {id} not found.");

                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating todo.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var deleted = await _todoService.DeleteTodo(id);

                if (!deleted)
                    return NotFound($"Todo with Id {id} not found.");

                return Ok(new
                {
                    Message = "Todo deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting todo.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }
    }
}