using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;
internal sealed class QueryCachingBehavior<TRequest, TResponse>(ICacheService cacheService, 
    ILogger<QueryCachingBehavior<TRequest, TResponse>> logger) : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string name = typeof(TRequest).Name;

        var result = await cacheService.GetAsync<TResponse>(request.CacheKey);

        if(result is not null)
        {
            logger.LogInformation("Cache hit for {Query}", name);
            return result;
        }

        logger.LogInformation("Cache miss for {Query}", name);

        var response = await next();

        if (response.IsSuccess)
        {
            await cacheService.SetAsync(request.CacheKey, response, request.Expiration, cancellationToken);
        }

        return response;
    }
}
