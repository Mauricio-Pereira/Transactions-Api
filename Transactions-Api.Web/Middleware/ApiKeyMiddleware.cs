using System.Text.Encodings.Web;
using System.Text.Json;
using Transactions_Api.Application.Services;

namespace Transactions_Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY_HEADER_NAME = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
        {
            if (RequerAutenticacao(context))
            {
                if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
                {
                    await ReturnUnauthorizedResponse(context, "API Key não fornecida.");
                    return; // Add 'return' here
                }

                var apiKey = await apiKeyService.GetByKeyAsync(extractedApiKey);

                if (apiKey == null)
                {
                    await ReturnUnauthorizedResponse(context, "API Key inválida.");
                    return; // Add 'return' here
                }

                // Optionally, you can set the API key in the context.Items collection
                // context.Items["ApiKey"] = apiKey;
            }

            await _next(context);
        }

        private bool RequerAutenticacao(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            // Define the paths that require authentication
            if (path.StartsWithSegments("/api/Transacoes"))
            {   
                    return true;
            }

            // Add more paths that require authentication here
            return false;
        }


        private async Task ReturnUnauthorizedResponse(HttpContext context, string message)
        {
            // Clear any content in the response
            context.Response.Clear();

            // Define the response status code and content type
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json; charset=utf-8";

            var result = JsonSerializer.Serialize(
                new { message },
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }
            );

            // Write the response using the UTF-8 encoding
            await context.Response.WriteAsync(result, System.Text.Encoding.UTF8);

        }
    }
}
