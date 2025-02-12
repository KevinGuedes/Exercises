using FluentResults;
using MediatR;
using Questao5.Application.Abstractions.Queries;

namespace Questao5.Application.Abstractions.Queries;

internal interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery, IBaseAppRequest
{
}