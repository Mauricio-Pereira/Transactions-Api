using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text.Json;
using Transactions_Api.Application.Services;
using Transactions_Api.Domain.Models;
using Transactions_Api.Shared.Exceptions;
using Transactions_Api.Middleware;

namespace Transactions_Api.Tests.Middleware
{
    public class ApiKeyMiddlewareTests
    {
        private readonly Mock<IApiKeyService> _apiKeyServiceMock;
        private readonly ApiKeyMiddleware _middleware;

        public ApiKeyMiddlewareTests()
        {
            _apiKeyServiceMock = new Mock<IApiKeyService>();
            _middleware = new ApiKeyMiddleware(next: (innerHttpContext) => Task.CompletedTask);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsUnauthorizedResponse_WhenApiKeyIsNotProvided()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/Transacoes";
            context.Request.Method = HttpMethods.Post;
            context.Response.Body = new MemoryStream(); // Adicione esta linha

            var apiKeyService = _apiKeyServiceMock.Object;

            // Act
            await _middleware.InvokeAsync(context, apiKeyService);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // Verifique se o responseBody não está vazio
            responseBody.Should().NotBeNullOrEmpty("O corpo da resposta não deve estar vazio.");

            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            response.Message.Should().Be("API Key não fornecida.");
        }
        
        [Fact]
        public async Task InvokeAsync_ReturnsUnauthorizedResponse_WhenApiKeyIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/Transacoes";
            context.Request.Method = HttpMethods.Post;
            context.Response.Body = new MemoryStream(); // Adicione esta linha

            var apiKeyService = _apiKeyServiceMock.Object;

            _apiKeyServiceMock.Setup(service => service.GetByKeyAsync(It.IsAny<string>()))
                .ReturnsAsync((ApiKey)null);

            context.Request.Headers.Add("X-API-KEY", "invalid-api-key");

            // Act
            await _middleware.InvokeAsync(context, apiKeyService);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // Verifique se o responseBody não está vazio
            responseBody.Should().NotBeNullOrEmpty("O corpo da resposta não deve estar vazio.");

            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            response.Message.Should().Be("API Key inválida.");
        }

        [Fact]
        public async Task InvokeAsync_ReturnAuthorizedResponse_WhenApiKeyIsValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/Transacoes";
            context.Request.Method = HttpMethods.Post;
            context.Response.Body = new MemoryStream(); // Adicione esta linha

            var apiKeyService = _apiKeyServiceMock.Object;

            _apiKeyServiceMock.Setup(service => service.GetByKeyAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApiKey { Id = 1, Key = "valid-api-key" });

            context.Request.Headers.Add("X-API-KEY", "valid api key");

            // Act
            await _middleware.InvokeAsync(context, apiKeyService);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        }
        

    }
}