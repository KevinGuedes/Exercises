using FluentResults;
using MediatR;

namespace Questao5.Application.Abstractions.Commands;

internal interface ICommand : IRequest<Result>, IBaseCommand, IBaseAppRequest
{
}

internal interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand, IBaseAppRequest
{
}