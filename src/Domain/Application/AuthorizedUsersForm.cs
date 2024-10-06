using System.Text.Json.Serialization;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public class AuthorizedUsersForm : ApplicationForm
{
    internal AuthorizedUsersForm() { }

    [JsonConstructor]
    internal AuthorizedUsersForm(List<AuthorizedUserRecord> authorizedUsers)
    {
        AuthorizedUsers = authorizedUsers;
    }

    public List<AuthorizedUserRecord> AuthorizedUsers { get; } = [];

    internal override bool IsVerified()
    {
        return AuthorizedUsers.Count > 0 && AuthorizedUsers.TrueForAll(x => x.IsVerified());
    }

    public Result<AuthorizedUserRecord> AddAuthorizedUser(string firstName, string lastName, string email,
        ApplicationDocument personalVerification, ApplicationDocument proofOfAddress)
    {
        if (AuthorizedUsers.Exists(x => x.Email == email))
            return Result.Fail(new EntityAlreadyExistsError(nameof(AuthorizedUserRecord),
                $"Authorized user with given user id {email} already exists"));

        var authorizedUserSectionRecord = new AuthorizedUserRecord(AuthorizedUserId.New(), firstName, lastName, email, personalVerification, proofOfAddress);

        AuthorizedUsers.Add(authorizedUserSectionRecord);

        return Result.Ok(authorizedUserSectionRecord);
    }

    internal override Result Verify(string segmentOrRecord, UserInfo userInfo)
    {
        var userId = AuthorizedUserId.Parse(segmentOrRecord);
        if (!AuthorizedUsers.Exists(x => x.Id == userId))
            return Result.Fail(new EntityNotFoundError(nameof(AuthorizedUserRecord)));

        AuthorizedUsers.First(x => x.Id == userId).Verify(userInfo);

        return Result.Ok();
    }

    internal override void RequireAdditionalInfo()
    {
        if (!IsVerified())
            return;

        foreach (var item in AuthorizedUsers)
        {
            item.RemoveVerificationFlag();
        }
    }

    internal bool RemoveDocument(DocumentId documentId)
    {
        var removed = false;

        foreach (var item in AuthorizedUsers)
        {
            if (item.PersonalVerificationDocument is not null && item.PersonalVerificationDocument.DocumentId == documentId)
            {
                item.PersonalVerificationDocument = null;
                removed = true;
                break;
            }
            else if (item.ProofOfAddressDocument is not null && item.ProofOfAddressDocument.DocumentId == documentId)
            {
                item.ProofOfAddressDocument = null;
                removed = true;
                break;
            }
        }

        return removed;
    }

    internal void Clear() => AuthorizedUsers.Clear();
}

public sealed record AuthorizedUserRecord : VerifiableRecord
{
    [JsonConstructor]
    internal AuthorizedUserRecord(AuthorizedUserId id,
        string firstName, string lastName, string email,
        ApplicationDocument? personalVerificationDocument,
        ApplicationDocument? proofOfAddressDocument,
        UserInfo? verifiedBy = null,
        DateTimeOffset? verifiedAt = null) : base(verifiedBy, verifiedAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PersonalVerificationDocument = personalVerificationDocument;
        ProofOfAddressDocument = proofOfAddressDocument;
    }

    public AuthorizedUserId Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public ApplicationDocument? PersonalVerificationDocument { get; internal set; }
    public ApplicationDocument? ProofOfAddressDocument { get; internal set; }
}
