using MediatR;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Application.Events;

public sealed class ApplicationApprovedEvent : INotification
{
    public ApplicationApprovedEvent(ApplicationId id, UserInfo userInfo)
    {
        Id = id;
        UserInfo = userInfo;
    }

    public ApplicationId Id { get; }
    public UserInfo UserInfo { get; }
}