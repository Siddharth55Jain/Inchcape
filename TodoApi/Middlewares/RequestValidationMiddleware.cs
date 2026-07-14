using System.Text;
using System.Text.RegularExpressions;

namespace TodoApi.Middlewares
{
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestValidationMiddleware> _logger;

        private static readonly string[] SuspiciousPatterns =
        {
            "<script",
            "</script>",
            "javascript:",
            "onerror=",
            "onload=",
            "drop table",
            "truncate table",
            "delete from",
            "insert into",
            "update ",
            "union select",
            "xp_cmdshell",
            "--",
            ";--",
            "/*",
            "*/",
            "@@",
            "char(",
            "nchar(",
            "varchar(",
            "cast(",
            "convert("
        };

        public RequestValidationMiddleware(
            RequestDelegate next,
            ILogger<RequestValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var url = context.Request.Path + context.Request.QueryString;

            if (ContainsMaliciousContent(url))
            {
                _logger.LogWarning("Blocked suspicious URL: {Url}", url);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsync("Invalid request.");

                return;
            }

            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();

                using (var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();

                    context.Request.Body.Position = 0;

                    if (ContainsMaliciousContent(body))
                    {
                        _logger.LogWarning("Blocked suspicious request body.");

                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        await context.Response.WriteAsync("Invalid request.");

                        return;
                    }
                }
            }

            await _next(context);
        }

        private static bool ContainsMaliciousContent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.ToLowerInvariant();

            return SuspiciousPatterns.Any(pattern =>
                input.Contains(pattern.ToLowerInvariant()));
        }
    }
}