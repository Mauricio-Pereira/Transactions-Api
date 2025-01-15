using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Transactions_Api.Controllers;

namespace Transactions_Api.Tests.Controllers;

public class DELETETransacoesControllerTests
{
    private readonly TransacoesController _controller;
    private readonly Mock<IMediator> _mockMediator;

    public DELETETransacoesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new TransacoesController(_mockMediator.Object);
    }


    [Fact]
    public async Task DeleteTransacao_ReturnsOkResult_WhenTransacaoExists()
    {
        // Arrange
        var txid = "txid123";
        var transacaoResponseDto = new TransacaoResponseDTO { Txid = txid, Valor = 100 };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoResponseDto);

        // Act
        var result = await _controller.DeleteTransacao(txid);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedTransacao = okResult.Value as TransacaoResponseDTO;
        returnedTransacao.Should().NotBeNull();
        returnedTransacao.Txid.Should().Be(txid);
        returnedTransacao.Valor.Should().Be(100);

        _mockMediator.Verify(m =>
                m.Send(
                    It.Is<DeleteTransactionCommand>(cmd => cmd.Txid == txid),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteTransacao_ThrowsNotFoundException_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Transação não encontrada"));

        // Act
        Func<Task> act = async () => await _controller.DeleteTransacao(txid);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Transação não encontrada");

        _mockMediator.Verify(m =>
                m.Send(
                    It.Is<DeleteTransactionCommand>(cmd => cmd.Txid == txid),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}