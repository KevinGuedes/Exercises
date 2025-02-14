using Bogus;
using Questao5.Application.Services;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.TestCommon.Extensions;

namespace Questao5.UnitTests.Services;
public sealed class SerializerServiceTests
{
    private readonly SerializerService _serializerService;
    private static readonly string[] _validTransferTypes = ["C", "D"];

    public SerializerServiceTests()
    {
        _serializerService = new SerializerService();
    }

    [Fact]
    public void Serialize_ShouldReturnSerializedString()
    {
        // Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .UseSeed(1)
            .Generate();

        // Act
        var result = _serializerService.Serialize(command);

        // Assert
        Assert.Equal("{\"Key\":\"3410cda1-5b13-a34e-6f84-a54adf7a0ea0\",\"AccountId\":\"8286d046-9740-a3e4-95cf-ff46699c73c4\",\"Value\":699.295832169846000,\"Type\":\"C\"}", result);
    }

    [Fact]
    public void Deserialize_ShouldReturnDeserializedObject()
    {
        // Arrange
        var serializedCommand = "{\"Key\":\"3410cda1-5b13-a34e-6f84-a54adf7a0ea0\",\"AccountId\":\"8286d046-9740-a3e4-95cf-ff46699c73c4\",\"Value\":699.295832169846000,\"Type\":\"C\"}";

        // Act
        var result = _serializerService.Deserialize<RegisterTransferCommand>(serializedCommand);

        // Assert
        Assert.Equal("3410cda1-5b13-a34e-6f84-a54adf7a0ea0", result.Key.ToString());
        Assert.Equal("8286d046-9740-a3e4-95cf-ff46699c73c4", result.AccountId.ToString());
        Assert.Equal(699.295832169846000m, result.Value);
        Assert.Equal("C", result.Type);
    }
}
