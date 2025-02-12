using FluentResults;
using MediatR;

namespace Questao5.Application.Abstractions.Queries;

internal interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery, IBaseAppRequest
{
}