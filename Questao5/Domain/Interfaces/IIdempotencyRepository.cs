using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface IIdempotencyRepository
{
    Task<Idempotency?> GetByKeyAsync(string key);
    Task InsertAsync(Idempotency Idempotency);
}
