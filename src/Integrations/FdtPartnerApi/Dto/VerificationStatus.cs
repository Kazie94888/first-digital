namespace SmartCoinOS.Integrations.FdtPartnerApi.Dto;

public enum VerificationStatus
{
    None = 1,
    Unverified = 2,
    Verified = 3,
    WaitingForReview = 4,
    ReviewInProgress = 5,
    Approved = 6,
    Declined = 7,
    Cancelled = 8
}