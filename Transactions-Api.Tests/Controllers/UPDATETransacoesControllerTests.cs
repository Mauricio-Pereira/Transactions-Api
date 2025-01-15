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

public class UPDATETransacoesControllerTests
{
    private readonly TransacoesController _controller;
    private readonly Mock<IMediator> _mockMediator;


    public UPDATETransacoesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new TransacoesController(_mockMediator.Object);
    }

    [Fact]
    public async Task UpdateTransacao_ReturnsOkResult_WhenUpdateIsSuccessful()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = txid, PagadorNome = "João" };
        var transacaoResource = new TransacaoResourceDTO
        {
            Transacao = new TransacaoResponseDTO { Txid = txid, PagadorNome = "João" },
            Links = new List<LinkDTO>()
        };

        // Configura o mock do Mediator para retornar o recurso esperado
        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoResource);

        // Configura o mock do IUrlHelper
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
            .Returns<string, object>((routeName, values) =>
            {
                var txid = values.GetType().GetProperty("txid")?.GetValue(values, null)?.ToString();
                return $"http://localhost:5000/api/transacoes/{txid}";
            });

        // Atribui o mock ao controller
        _controller.Url = mockUrlHelper.Object;

        // Act
        var result = await _controller.UpdateTransacao(txid, transacaoUpdateDto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedTransacao = okResult.Value as TransacaoResourceDTO;
        returnedTransacao.Should().NotBeNull();
        returnedTransacao.Transacao.Txid.Should().Be(txid);
        returnedTransacao.Transacao.PagadorNome.Should().Be("João");

        // Verifica se o link foi gerado corretamente
        returnedTransacao.Links.Should().ContainSingle(link =>
            link.Rel == "self" &&
            link.Href == $"http://localhost:5000/api/transacoes/{txid}" &&
            link.Method == "GET"
        );
    }


    [Fact]
    public async Task UpdateTransacao_ReturnsBadRequest_WhenTxidIsDifferentFromTransacaoUpdateDto()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = "txid456" };
        var exceptionMessage = "O Txid fornecido na URL não corresponde ao Txid do corpo da requisição.";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<BadRequestException>(() => _controller.UpdateTransacao(txid, transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task UpdateTransacao_ThrowsNotFoundException_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = txid };
        var exceptionMessage = "Transação não encontrada";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateTransacao(txid, transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task UpdateTransacao_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = txid };
        var exceptionMessage = "Erro inesperado";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateTransacao(txid, transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }
}