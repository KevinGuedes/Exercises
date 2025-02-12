using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces;

public interface ITransferRepository
{
    Task InsertAsync(Transfer transfer);
}
