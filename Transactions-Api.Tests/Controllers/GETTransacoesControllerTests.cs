using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Transactions_Api.Controllers;

namespace Transactions_Api.Tests.Controllers;

public class GETTransacoesControllerTests
{
    private readonly TransacoesController _controller;
    private readonly Mock<IMediator> _mockMediator;

    public GETTransacoesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new TransacoesController(_mockMediator.Object);
    }


    [Fact]
    public async Task GetTransacoes_ReturnsOkResult_WhenTransacoesExist()
    {
        // Arrange
        var transacoesResourceDto = new List<TransacaoResourceDTO>
        {
            new TransacaoResourceDTO
            {
                Transacao = new TransacaoResponseDTO { Txid = "txid1", Valor = 100 },
                Links = new List<LinkDTO>()
            },
            new TransacaoResourceDTO
            {
                Transacao = new TransacaoResponseDTO { Txid = "txid2", Valor = 200 },
                Links = new List<LinkDTO>()
            }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllTransacoesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoesResourceDto);

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
        var result = await _controller.GetTransacoes();

        // Assert
        var okResult = result.Result as OkObjectResult; // Use `.Result` para acessar o valor de ActionResult<T>
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedTransacoes = okResult.Value as IEnumerable<TransacaoResourceDTO>;
        returnedTransacoes.Should().NotBeNull();
        returnedTransacoes.Should().HaveCount(2);

        // Verifica os links sem duplicação
        returnedTransacoes.First().Links.Should().ContainSingle(link =>
            link.Rel == "self" &&
            link.Href == "http://localhost:5000/api/transacoes/txid1" &&
            link.Method == "GET");

        returnedTransacoes.Last().Links.Should().ContainSingle(link =>
            link.Rel == "self" &&
            link.Href == "http://localhost:5000/api/transacoes/txid2" &&
            link.Method == "GET");
    }


    [Fact]
    public async Task GetTransacoes_ReturnsNotFound_WhenNoTransacoesExist()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllTransacoesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TransacaoResourceDTO>());

        // Act
        var result = await _controller.GetTransacoes();

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult; // Use `.Result`
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be("Nenhuma transação encontrada");
    }

    [Fact]
    public async Task GetTransacaoByTxid_ReturnsOkResult_WhenTransacaoExists()
    {
        // Arrange
        var txid = "txid123";
        var transacaoResource = new TransacaoResourceDTO
        {
            Transacao = new TransacaoResponseDTO { Txid = txid, Valor = 100 },
            Links = new List<LinkDTO>()
        };

        // Mock do Mediator para retornar o TransacaoResourceDTO
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTransacaoByTxidQuery>(), It.IsAny<CancellationToken>()))
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
        var result = await _controller.GetTransacaoByTxid(txid);

        // Assert
        var okResult = result.Result as OkObjectResult; // Use `.Result`
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedResource = okResult.Value as TransacaoResourceDTO;
        returnedResource.Should().NotBeNull();
        returnedResource.Transacao.Txid.Should().Be(txid);

        // Verifica se o link foi adicionado corretamente
        returnedResource.Links.Should().ContainSingle(link =>
            link.Rel == "self" &&
            link.Href == $"http://localhost:5000/api/transacoes/{txid}" &&
            link.Method == "GET"
        );
    }


    [Fact]
    public async Task GetTransacaoByTxid_ReturnsNotFound_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetTransacaoByTxidQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransacaoResourceDTO)null);

        // Act
        var result = await _controller.GetTransacaoByTxid(txid);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult; // Use `.Result`
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be("Transação não encontrada");
    }
}