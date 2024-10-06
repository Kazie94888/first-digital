using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class MintOrderTests : BaseOrderTest
{
    const string _safeTxHash = "0x421558f7f5590b23442c41CE1e5382Bf901a890C";

    [Fact]
    public void Signed_WhenOk_SetsOrderProperties()
    {
        var order = GetMintOrderFromFactory()
                        .EnrichWithRsnReference()
                        .EnrichWithRsnAmount();

        const string safeTxHash = "0x421558f7f5590b23442c41CE1e5382Bf901a890C";
        var dateOfSign = new DateTime(2024, 01, 05);
        var signatures = new List<SafeSignature>
        {
            new("0xA29773B3a967681322a20f55510767f9459646FA","John Smith", new DateTimeOffset(dateOfSign)),
            new("0xB39847B3a967681322a20f55510767f9459646FB","Jane Doe", new DateTimeOffset(dateOfSign)),
            new("0xC39847B3a967681322a20f55510767f9459646FC","Bob Johnson", new DateTimeOffset(dateOfSign))
        };

        order.Signed(safeTxHash, signatures, UserInfo);

        Assert.Equal(safeTxHash, order.SafeTxHash);
        Assert.Equal(OrderProcessingStatus.SigningInitiated, order.ProcessingStatus);

        var signingInitiatedEvent = order.DomainEvents.OfType<SigningInitiatedEvent>().SingleOrDefault();
        Assert.NotNull(signingInitiatedEvent);
        Assert.Equal(signatures[0].Address, signingInitiatedEvent.Parameters.First(p => p.Name == nameof(SafeSignature.Address)).Value);
        Assert.Equal(signatures[0], order.Signatures[0]);

        var signingOccurredEvents = order.DomainEvents.OfType<SigningOccurredEvent>().ToList();
        Assert.Equal(2, signingOccurredEvents.Count);
        Assert.Contains(signingOccurredEvents, x => x.Parameters.Any(p => p.Name == nameof(SafeSignature.Address) && p.Value == signatures[1].Address));
        Assert.Contains(signingOccurredEvents, x => x.Parameters.Any(p => p.Name == nameof(SafeSignature.Address) && p.Value == signatures[2].Address));
        Assert.Equal(signatures[1], order.Signatures[1]);
        Assert.Equal(signatures[2], order.Signatures[2]);
    }

    [Fact]
    public void Signed_WhenOneSignature_SigningInitiatedEventEmitted()
    {
        var order = GetMintOrderFromFactory()
                        .EnrichWithRsnReference()
                        .EnrichWithRsnAmount();


        var dateOfSign = new DateTime(2024, 01, 05);
        var signatures = new List<SafeSignature>
        {
            new("0xA29773B3a967681322a20f55510767f9459646FA","John Smith", new DateTimeOffset(dateOfSign))
        };

        var signedResult = order.Signed(_safeTxHash, signatures, UserInfo);

        var signingInitiatedEvent = order.DomainEvents.OfType<SigningInitiatedEvent>().SingleOrDefault();
        Assert.NotNull(signingInitiatedEvent);
        Assert.Equal(signatures[0].Address, signingInitiatedEvent.Parameters.First(p => p.Name == nameof(SafeSignature.Address)).Value);

        var signingOccurredEvents = order.DomainEvents.OfType<SigningOccurredEvent>().ToList();
        Assert.True(signedResult.IsSuccess);
        Assert.Empty(signingOccurredEvents);
    }

    [Fact]
    public void Signed_WhenMultipleSequentialSignature_ReturnsOk()
    {
        var order = GetMintOrderFromFactory().EnrichWithRsnReference().EnrichWithRsnAmount();

        var dateOfSign = new DateTime(2024, 01, 05);
        var signatureOne = new SafeSignature("0xA29773B3a967681322a20f55510767f9459646FA", "John Smith", new DateTimeOffset(dateOfSign));
        var signatureTwo = new SafeSignature("0xA3a967681322a20f55510767f9459646FA29773B3", "Johns Wife", new DateTimeOffset(dateOfSign));

        var signedResultOne = order.Signed(_safeTxHash, [signatureOne], UserInfo);
        var statusToSignInitiated = order.ProcessingStatus == OrderProcessingStatus.SigningInitiated;

        var signedResultTwo = order.Signed(_safeTxHash, [signatureTwo], UserInfo);

        Assert.True(signedResultOne.IsSuccess);
        Assert.True(statusToSignInitiated);
        Assert.True(signedResultTwo.IsSuccess);
    }

    [Fact]
    public void MintExecuted_WhenMintOrder_SetsStatusToCompleted()
    {
        var order = GetMintOrderFromFactory()
                                .EnrichWithRsnReference()
                                .EnrichWithRsnAmount()
                                .EnrichWithSignature();

        var executeResult = order.Executed(Signature, UserInfo);

        Assert.True(executeResult.IsSuccess);
        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void MintExecuted_WhenOk_SetsOrderProperties()
    {
        var order = GetMintOrderFromFactory()
                            .EnrichWithRsnReference()
                            .EnrichWithRsnAmount()
                            .EnrichWithSignature();

        var executeResult = order.Executed(Signature, UserInfo);

        Assert.True(executeResult.IsSuccess);
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(OrderProcessingStatus.TransactionExecuted, order.ProcessingStatus);
        Assert.Contains(order.DomainEvents, x => x is MintTransactionExecutedEvent);
    }

    [Fact]
    public void MintExecuted_WhenStatusNotInProgress_FailsWithError()
    {
        var order = GetMintOrderFromFactory();

        var executeResult = () => order.Executed(Signature, UserInfo);

        Assert.Throws<EntityInvalidStateException>(executeResult);
    }

    [Fact]
    public void MintExecuted_WhenNotSigned_FailsWithError()
    {
        var order = GetMintOrderFromFactory()
                            .EnrichWithRsnReference()
                            .EnrichWithRsnAmount();

        var executeResult = order.Executed(Signature, UserInfo);

        Assert.True(executeResult.IsFailed);
        Assert.True(executeResult.HasError<EntityInvalidOperation>());
    }
}