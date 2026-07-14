namespace TodoApi.Middlewares
{
    public static class RequestValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestValidation(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}
