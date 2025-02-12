using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<Idempotency?> GetByKeyAsync(Guid key);
    Task InsertAsync(Idempotency Idempotency);
}
