namespace Transactions_Api.Infrastructure.Repositories;

public interface IUnitOfWork
{
    ITransacaoRepository TransacaoRepository { get; }
    IApiKeyRepository ApiKeyRepository { get; }
    Task<bool> Commit();
    void Dispose();
}