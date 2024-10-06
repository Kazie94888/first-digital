using FluentResults;
using MediatR;

namespace SmartCoinOS.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
