namespace Questao5.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public long Number { get; private set; }
    public string HolderName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    /// <summary>
    /// Parameterless constructor required for ORM and fakers
    /// </summary>
    private Account()
    {
    }

    public Account(string holderName, bool isActive)
    {
        Id = Guid.NewGuid();
        Number = new Random().Next();
        HolderName = holderName;
        IsActive = isActive;
    }
}
