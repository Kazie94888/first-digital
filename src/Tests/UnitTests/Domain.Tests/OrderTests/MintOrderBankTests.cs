using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class MintOrderBankTests : BaseOrderTest
{
    [Fact]
    public void CreateDepositInstruction_WhenOk_SetsOrderProperties()
    {
        const int instructionId = 777;
        const string instructionRefNumber = "CMR-GUM8DP";

        var order = GetBankMintOrderFromFactory();
        var setInstrResult = order.CreateDepositInstruction(instructionId, instructionRefNumber);

        var procStatusIsDepositInstrCreated = order.ProcessingStatus == OrderProcessingStatus.DepositInstructionCreated;

        Assert.True(setInstrResult.IsSuccess);
        Assert.True(procStatusIsDepositInstrCreated);
        Assert.NotNull(order.DepositInstruction);
        Assert.Equal(instructionId, order.DepositInstruction.Id);
        Assert.Equal(instructionRefNumber, order.DepositInstruction.ReferenceNumber);
    }

    [Fact]
    public void CreateDepositInstruction_WhenDepositExists_FailsWithError()
    {
        var order = GetBankMintOrderFromFactory();
        var setInstrSuccess = order.CreateDepositInstruction(111, "CMR-GUM8DP");
        var setInstrError = order.CreateDepositInstruction(111, "CMR-GUM8DP");

        Assert.True(setInstrSuccess.IsSuccess);
        Assert.True(setInstrError.IsFailed);
    }

    [Fact]
    public void CompleteDepositInstruction_WhenOk_SetsOrderProperties()
    {
        const int instructionId = 777;
        const string instructionRefNumber = "CMR-GUM8DP";

        var order = GetBankMintOrderFromFactory();
        var actualAmount = new Money
        {
            Amount = 100,
            Currency = order.OrderedAmount.Currency
        };
        var setInstrResult = order.CreateDepositInstruction(instructionId, instructionRefNumber);
        var completeInstrResult = order.CompleteDepositInstruction([actualAmount], UserInfo);

        Assert.True(setInstrResult.IsSuccess);
        Assert.True(completeInstrResult.IsSuccess);
        Assert.NotNull(order.ActualAmount);
        Assert.Equal(actualAmount, order.ActualAmount);
        Assert.Equal(OrderProcessingStatus.DepositInstructionCompleted, order.ProcessingStatus);
    }

    [Fact]
    public void CompleteDepositInstruction_WhenDuplicateInvoke_FailsWithError()
    {
        var order = GetBankMintOrderFromFactory();
        var actualAmount = new Money
        {
            Amount = 100,
            Currency = "USD"
        };

        var createInstr = order.CreateDepositInstruction(1, "a");
        var complSuccess = order.CompleteDepositInstruction([actualAmount], UserInfo);
        var complError = order.CompleteDepositInstruction([actualAmount], UserInfo);

        Assert.True(createInstr.IsSuccess);
        Assert.True(complSuccess.IsSuccess);
        Assert.True(complError.IsFailed);
    }

    [Fact]
    public void CompleteDepositInstruction_WhenTransactionsPassed_CalculatedCorrectly()
    {
        var order = GetBankMintOrderFromFactory();

        var transactions = new List<Money>
        {
            new()
            {
                Amount = 1_000_000,
                Currency = "USD"
            },
            new()
            {
                Amount = -500,
                Currency = "USD"
            },
        };

        var createInstr = order.CreateDepositInstruction(1, "a");
        var complSuccess = order.CompleteDepositInstruction(transactions, UserInfo);

        var expected = 1_000_000 - 500;

        Assert.True(createInstr.IsSuccess);
        Assert.True(complSuccess.IsSuccess);
        Assert.NotNull(order.ActualAmount);
        Assert.Equal(expected, order.ActualAmount.Amount);
    }

    [Fact]
    public void CompleteDepositInstruction_WhenAmountZeroOrLess_FailsWithError()
    {
        var order = GetBankMintOrderFromFactory();

        var createInstr = order.CreateDepositInstruction(1, "abc-123");
        var complResult = order.CompleteDepositInstruction([], UserInfo);

        Assert.True(createInstr.IsSuccess);
        Assert.True(complResult.IsFailed);
    }
}
