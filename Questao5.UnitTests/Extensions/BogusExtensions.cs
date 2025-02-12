using Bogus;

namespace Questao5.UnitTests.Extensions;

internal static class BogusExtensions
{
    public static Faker<T> UsePrivateConstructor<T>(this Faker<T> faker) where T : class
    {
        var instance = Activator.CreateInstance(typeof(T), nonPublic: true) as T;
        return faker.CustomInstantiator(f => instance!);
    }
}
