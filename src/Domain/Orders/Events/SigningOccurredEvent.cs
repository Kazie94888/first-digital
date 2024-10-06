﻿using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public sealed class SigningOccurredEvent : OrderAuditEventBase
{
    public SigningOccurredEvent(SafeSignature signature, OrderId orderId, ClientId clientId, UserInfo auditedBy) : base(
        orderId, clientId, auditedBy)
    {
        Parameters.Add(new AuditLogParameter(nameof(SafeSignature.Address), signature.Address));
        Parameters.Add(new AuditLogParameter(nameof(SafeSignature.SubmissionDate),
            signature.SubmissionDate.ToUnixTimeSeconds().ToString()));


        var signer = signature.Alias ?? signature.Address;
        Signer = signer;

        Parameters.Add(new AuditLogParameter(nameof(SafeSignature.Alias), signature.Alias ?? signature.Address));
    }

    private string Signer { get; }

    public override string GetDescription()
    {
        return $"Signed by {Signer}";
    }
}