namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public enum PaymentInstructionStatus
{
    Initiated = 1,
    Pending = 2,
    Completed = 3,
    Rejected = 4,
    Cancelled = 5,
    WaitingForReview = 6,
    Failed = 7,
}
