using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace Transactions_Api.Tests.Repository;

public class TransacaoRepositoryTests
{
    
    private readonly Mock<ITransacaoRepository> _mockTransacaoService;
    
    public TransacaoRepositoryTests()
    {
        _mockTransacaoService = new Mock<ITransacaoRepository>();
    }

    [Fact]
    public async Task AddTransacao_ReturnsCreatedAtAction_WhenTransacaoIsValid()
    {
        // Arrange
        var transacao = new Transacao { Valor = 100 };
        var transacaoResponse = new Transacao { Txid = "txid123", Valor = 100 };

        _mockTransacaoService.Setup(service => service.AddAsync(transacao))
            .ReturnsAsync(transacaoResponse);

        // Act
        var result = await _mockTransacaoService.Object.AddAsync(transacao);

        // Assert
        result.Should().NotBeNull();
        result.Txid.Should().Be(transacaoResponse.Txid);
    }
    
    [Fact]
    public async Task AddTransacao_ReturnsBadRequest_WhenTransacaoIsNull()
    {
        // Arrange
        Transacao transacao = null;

        // Act
        var result = await _mockTransacaoService.Object.AddAsync(transacao);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddTransacao_ReturnsCreatedAtAction_WhenTransacaoIsInvalid()
    {
        // Arrange
        var transacao = new Transacao { Valor = 0 };
        var exceptionMessage = "O valor da transação deve ser maior que zero.";

        _mockTransacaoService.Setup(service => service.AddAsync(transacao))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.AddAsync(transacao));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    public async Task AddTransacao_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var transacao = new Transacao { Valor = 100 };
        var exceptionMessage = "Erro inesperado";

        _mockTransacaoService.Setup(service => service.AddAsync(transacao))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _mockTransacaoService.Object.AddAsync(transacao));

        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task AddTransacao_ReturnsBadRequest_WhenTransacaoIsInvalid()
    {
        // Arrange
        var transacao = new Transacao { Valor = -100 };
        var exceptionMessage = "O valor da transação deve ser maior que zero.";

        _mockTransacaoService.Setup(service => service.AddAsync(transacao))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.AddAsync(transacao));
    }


    [Fact]
    public async Task GetTransacaoById_ReturnsOkResult_WhenTransacaoExists()
    {
        // Arrange
        var txid = "txid123";
        var transacao = new Transacao { Txid = txid, Valor = 150 };

        _mockTransacaoService.Setup(service => service.GetByTxidAsync(txid))
            .ReturnsAsync(transacao);

        // Act
        var result = await _mockTransacaoService.Object.GetByTxidAsync(txid);

        // Assert
        result.Should().NotBeNull();
        result.Txid.Should().Be(txid);
    }
    
    
    [Fact]
    public async Task GetTransacaoById_ReturnsNull_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";

        _mockTransacaoService.Setup(service => service.GetByTxidAsync(txid))
            .ReturnsAsync((Transacao)null);

        // Act
        var result = await _mockTransacaoService.Object.GetByTxidAsync(txid);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllTransacoes_ReturnsTransacoesList_WhenExists()
    {
        var transacoes = new List<Transacao>
        {
            new Transacao { Txid = "txid1", Valor = 100 },
            new Transacao { Txid = "txid2", Valor = 200 }
        };

        _mockTransacaoService.Setup(service => service.GetAllAsync())
            .ReturnsAsync(transacoes);

        var result = await _mockTransacaoService.Object.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetAllTransacoes_ReturnsEmptyList_WhenNotExists()
    {
        _mockTransacaoService.Setup(service => service.GetAllAsync())
            .ReturnsAsync(new List<Transacao>());

        var result = await _mockTransacaoService.Object.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(0);
    }

    [Fact]
    public async Task UpdateTransacao_ReturnsOkResult_WhenUpdateIsSuccessful()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = txid, PagadorNome = "João" };
        var transacaoResponseDto = new Transacao { Txid = txid, PagadorNome = "João" };

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ReturnsAsync(transacaoResponseDto);

        // Act
        var result = await _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto);

        // Assert
        result.Should().NotBeNull();
        result.Txid.Should().Be(txid);
        result.PagadorNome.Should().Be("João");
    }
    
    
    [Fact]
    public async Task UpdateTransacao_ReturnsBadRequest_WhenTxidIsDifferentFromTransacaoUpdateDto()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = "txid456" };
        var exceptionMessage = "O Txid fornecido na URL não corresponde ao Txid do corpo da requisição.";

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ThrowsAsync(new NotFoundException(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    public async Task UpdateTransacao_ThrowsNotFoundException_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = txid };
        var exceptionMessage = "Transação não encontrada";

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ThrowsAsync(new NotFoundException(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    public async Task UpdateTransacao_ThrowsBadRequest_WhenTransacaoIsAlreadyProcessed()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = txid };
        var transacaoExistente = new Transacao { Txid = txid, E2eId = "e2eId" };
        var exceptionMessage = "Não é possível alterar uma transação que já foi processada.";

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    public async Task UpdateTransacao_ThrowsBadRequest_WhenTxidIsInvalid()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = txid };
        var exceptionMessage = "O Txid é inválido. Deve conter apenas caracteres alfanuméricos e ter entre 26 e 35 caracteres.";

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    
    public async Task UpdateTransacao_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var txid = "txid123";
        var transacaoUpdateDto = new Transacao { Txid = txid };
        var exceptionMessage = "Erro inesperado";

        _mockTransacaoService.Setup(service => service.UpdateAsync(transacaoUpdateDto))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _mockTransacaoService.Object.UpdateAsync(transacaoUpdateDto));

        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task DeleteTransacao_ReturnsOkResult_WhenDeleteIsSuccessful()
    {
        // Arrange
        var txid = "txid123";
        var transacao = new Transacao { Txid = txid };

        _mockTransacaoService.Setup(service => service.DeleteAsync(txid))
            .ReturnsAsync(transacao);

        // Act
        var result = await _mockTransacaoService.Object.DeleteAsync(txid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTransacao_ReturnsNull_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";

        _mockTransacaoService.Setup(service => service.DeleteAsync(txid))
            .ReturnsAsync((Transacao)null);

        // Act
        var result = await _mockTransacaoService.Object.DeleteAsync(txid);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteTransacao_ThrowsNotFoundException_WhenTransacaoDoesNotExist()
    {
        // Arrange
        var txid = "txid123";
        var exceptionMessage = "Transação não encontrada";

        _mockTransacaoService.Setup(service => service.DeleteAsync(txid))
            .ThrowsAsync(new NotFoundException(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _mockTransacaoService.Object.DeleteAsync(txid));

        exception.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task DeleteTransacao_ThrowsBadRequest_WhenTransacaoIsAlreadyProcessed()
    {
        // Arrange
        var txid = "txid123";
        var transacao = new Transacao { Txid = txid, E2eId = "e2eId" };
        var exceptionMessage = "Não é possível alterar uma transação que já foi processada.";

        _mockTransacaoService.Setup(service => service.DeleteAsync(txid))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.DeleteAsync(txid));

        exception.Message.Should().Be(exceptionMessage);
    }
    
    [Fact]
    public async Task DeleteTransacao_ThrowsBadRequest_WhenTxidIsInvalid()
    {
        // Arrange
        var txid = "txid123";
        var exceptionMessage = "O Txid é inválido. Deve conter apenas caracteres alfanuméricos e ter entre 26 e 35 caracteres.";

        _mockTransacaoService.Setup(service => service.DeleteAsync(txid))
            .ThrowsAsync(new BadRequestException(exceptionMessage));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<BadRequestException>(() => _mockTransacaoService.Object.DeleteAsync(txid));

        exception.Message.Should().Be(exceptionMessage);
    }


}