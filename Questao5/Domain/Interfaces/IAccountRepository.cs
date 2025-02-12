using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId);
    Task<decimal> GetBalanceAsync(Guid accountId);
}
