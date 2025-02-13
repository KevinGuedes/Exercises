using Bogus;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Domain.Entities;
using Questao5.TestCommon.Extensions;

namespace Questao5.TestCommon.TestData;

public static class AccountTestData
{
    public static GetAccountBalanceQuery CreateGetAccountBalanceQuery(
        Guid? accountId,
        bool useDefaultSeed = true)
    {
        accountId ??= Guid.NewGuid();

        var faker = new Faker<GetAccountBalanceQuery>()
           .UsePrivateConstructor()
           .RuleFor(query => query.AccountId, f => f.Random.Guid());

        if (useDefaultSeed)
            faker.UseSeed(12);

        return faker.Generate();
    }

    public static GetAccountBalanceQuery CreateGetAccountBalanceQueryForInvalidAccount()
        => new Faker<GetAccountBalanceQuery>()
            .UsePrivateConstructor()
            .RuleFor(query => query.AccountId, Guid.Empty)
            .Generate();

    public static Account CreateActiveAccount(bool useDefaultSeed = true)
    {
        var faker = new Faker<Account>()
           .UsePrivateConstructor()
           .RuleFor(account => account.Id, f => f.Random.Guid())
           .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
           .RuleFor(account => account.HolderName, f => f.Person.FullName)
           .RuleFor(account => account.IsActive, true);

        if (useDefaultSeed)
            faker.UseSeed(1);

        return faker.Generate();
    }

    public static Account CreateInactiveAccount(bool useDefaultSeed = true)
    {
        var faker = new Faker<Account>()
           .UsePrivateConstructor()
           .RuleFor(account => account.Id, f => f.Random.Guid())
           .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
           .RuleFor(account => account.HolderName, f => f.Person.FullName)
           .RuleFor(account => account.IsActive, false);

        if (useDefaultSeed)
            faker.UseSeed(2);

        return faker.Generate();
    }
}
