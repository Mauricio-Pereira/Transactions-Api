using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Transactions_Api.Controllers;

namespace Transactions_Api.Tests.Controllers
{
    public class CREATETransacoesControllerTests
    {
        private readonly TransacoesController _controller;
        private readonly Mock<IMediator> _mockMediator;

        public CREATETransacoesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new TransacoesController(_mockMediator.Object);
        }

        [Fact]
        public async Task AddTransacao_ReturnsCreatedAtAction_WhenTransacaoIsValid()
        {
            // Arrange
            var transacaoCreateDto = new TransacaoCreateRequestDTO { Valor = 100 };
            var transacaoResponse = new TransacaoResponseCreateDTO { Txid = "txid123", Valor = 100 };

            // Configura o mediator para retornar uma TransacaoResponseCreateDTO
            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transacaoResponse);

            // Act
            var result = await _controller.AddTransacao(transacaoCreateDto);

            // Assert
            // O método AddTransacao retorna `IActionResult`, verifique o tipo real
            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetTransacaoByTxid));
            createdAtActionResult.RouteValues["txid"].Should().Be(transacaoResponse.Txid);

            var returnedTransacao = createdAtActionResult.Value as TransacaoResponseCreateDTO;
            returnedTransacao.Should().NotBeNull();
            returnedTransacao.Txid.Should().Be(transacaoResponse.Txid);

            // Verifica se o Mediator foi chamado com um CreateTransactionCommand cujo Valor = 100
            _mockMediator.Verify(m =>
                    m.Send(
                        It.Is<CreateTransactionCommand>(cmd => cmd.Valor == 100),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }


        [Fact]
        public async Task AddTransacao_ThrowsBadRequestException_WhenTransacaoCreateDtoIsNull()
        {
            // Arrange
            var exceptionMessage = "Transação inválida";

            // Simula que o handler lança a exceção:
            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException(exceptionMessage));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => _controller.AddTransacao(null));
            ex.Message.Should().Be(exceptionMessage);
        }


        [Fact]
        public async Task AddTransacao_ThrowsBadRequestException_WhenTransacaoCreateDtoIsInvalid()
        {
            // Arrange
            var transacao = new TransacaoCreateRequestDTO { Valor = -100 };
            var exceptionMessage = "O valor da transação deve ser maior que zero.";

            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException(exceptionMessage));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => _controller.AddTransacao(transacao));
            ex.Message.Should().Be(exceptionMessage);
        }


        [Fact]
        public async Task AddTransacao_HandlesCachingFailure_Gracefully()
        {
            // Arrange
            var transacaoCreateDto = new TransacaoCreateRequestDTO { Valor = 100 };
            var transacaoResponse = new TransacaoResponseCreateDTO { Txid = "txid123", Valor = 100 };

            // Simulando que o handler lida com a falha de cache internamente e 
            // ainda retorna o transacaoResponse
            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transacaoResponse);

            // Act
            var result = await _controller.AddTransacao(transacaoCreateDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetTransacaoByTxid));
            createdAtActionResult.RouteValues["txid"].Should().Be(transacaoResponse.Txid);

            var returnedTransacao = createdAtActionResult.Value as TransacaoResponseCreateDTO;
            returnedTransacao.Should().NotBeNull();
            returnedTransacao.Txid.Should().Be(transacaoResponse.Txid);

            // Verifica se o mediator foi chamado
            _mockMediator.Verify(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}