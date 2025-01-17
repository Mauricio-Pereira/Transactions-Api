using AutoMapper;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Exceptions;
using Transactions_Api.Shared.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Transactions_Api.Tests.Services;

public class TransacaoServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITransacaoRepository> _mockTransacaoRepository;
    private readonly IMapper _mapper;
    private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
    private readonly Mock<ITxidGenerator> _mockTxidGenerator = new Mock<ITxidGenerator>();
    private readonly TransacaoService _service;
    private readonly Mock<ILogger<TransacaoService>> _mockLogger = new Mock<ILogger<TransacaoService>>();


    public TransacaoServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTransacaoRepository = new Mock<ITransacaoRepository>();
        

        // Set up the UnitOfWork to return the mock repository
        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository)
            .Returns(_mockTransacaoRepository.Object);
        
        

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TransacaoCreateRequestDTO, Transacao>();
            cfg.CreateMap<Transacao, TransacaoResponseCreateDTO>();
            cfg.CreateMap<TransacaoUpdateDTO, Transacao>();
            cfg.CreateMap<Transacao, TransacaoResponseDTO>();
        });
        _mapper = mapperConfig.CreateMapper();

        _service = new TransacaoService(_mockUnitOfWork.Object, _mapper, _mockTxidGenerator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTransacoesList()
    {
        var transacoes = new List<Transacao>
        {
            new Transacao { Txid = "txid1", Valor = 100 },
            new Transacao { Txid = "txid2", Valor = 200 }
        };

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetAllAsync())
            .ReturnsAsync(transacoes);

        var result = await _service.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByTxidAsync_ShouldReturnTransacao_WhenExists()
    {
        var txid = "txid123";
        var transacao = new Transacao { Txid = txid, Valor = 150 };

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync(transacao);

        var result = await _service.GetByTxidAsync(txid);

        result.Should().NotBeNull();
        result.Txid.Should().Be(txid);
    }

    [Fact]
    public async Task GetByTxidAsync_ShouldReturnNull_WhenNotExists()
    {
        var txid = "txid123";

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync((Transacao)null);

        var result = await _service.GetByTxidAsync(txid);

        result.Should().BeNull();
    }


    [Fact]
    public async Task AddAsync_ShouldThrowBadRequest_WhenValorInvalid()
    {
        var transacaoCreateDto = new TransacaoCreateRequestDTO { Valor = -100 };

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.AddAsync(transacaoCreateDto));
    }

    [Fact]
    public async Task AddAsync_ShouldAddTransacao_WhenValid()
    {
        // Arrange
        var txidEsperado = "generatedTxid";
        var transacaoCreateDto = new TransacaoCreateRequestDTO { Valor = 100 };

        // Mock para ITxidGenerator
        var mockTxidGenerator = new Mock<ITxidGenerator>();
        mockTxidGenerator.Setup(x => x.GerarTxid()).Returns(txidEsperado);

        // Instância do serviço com os mocks injetados
        var service = new TransacaoService(_mockUnitOfWork.Object, _mapper, mockTxidGenerator.Object, _mockLogger.Object);

        // Act
        var result = await service.AddAsync(transacaoCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Txid.Should().Be(txidEsperado); // Verifica se o Txid corresponde ao valor esperado
    }




    [Fact]
    public async Task UpdateAsync_ShouldThrowBadRequest_WhenTxidMismatch()
    {
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = "txid456" };

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.UpdateAsync(txid, transacaoUpdateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFound_WhenTransacaoNotFound()
    {
        // Arrange
        var txid = "00000000000000000000000000"; // Txid com 26 caracteres alfanuméricos válidos
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = txid };

        // Configura o mock para retornar null, simulando que a transação não foi encontrada
        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync((Transacao)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateAsync(txid, transacaoUpdateDto));
    }



    [Fact]
    public async Task UpdateAsync_ShouldThrowBadRequest_WhenAlreadyProcessed()
    {
        var txid = "txid123";
        var transacaoUpdateDto = new TransacaoUpdateDTO { Txid = txid };
        var transacaoExistente = new Transacao { Txid = txid, E2eId = "e2eId" };

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync(transacaoExistente);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.UpdateAsync(txid, transacaoUpdateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTransacao_WhenExists()
    {
        var txid = "validtxid1234567890123456"; // txid válido com 26 caracteres
        var transacao = new Transacao { Txid = txid };

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync(transacao);

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.DeleteAsync(txid))
            .ReturnsAsync(transacao);

        var result = await _service.DeleteAsync(txid);

        result.Should().NotBeNull();
        result.Txid.Should().Be(txid);

        _mockUnitOfWork.Verify(uow => uow.TransacaoRepository.DeleteAsync(txid), Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_ShouldReturnNull_WhenTransacaoNotFound()
    {
        var txid = "txid123";

        _mockUnitOfWork.Setup(uow => uow.TransacaoRepository.GetByTxidAsync(txid))
            .ReturnsAsync((Transacao)null);

        var result = await _service.DeleteAsync(txid);

        result.Should().BeNull();
    }
}