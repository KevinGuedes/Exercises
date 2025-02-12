namespace Questao5.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public long Number { get; private set; }
    public string HolderName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public Account()
    {
    }

    public Account(Guid id, string holderName, bool isActive)
    {
        Id = id;
        Number = new Random().Next();
        HolderName = holderName;
        IsActive = isActive;
    }
}
