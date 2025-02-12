using FluentResults;
using MediatR;

namespace Questao5.Application.Abstractions.Queries;

internal interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}