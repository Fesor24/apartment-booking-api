using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors
{
    internal class LoggingBehavior<TRequest, TResponse>(ILogger logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var name = request.GetType().Name;

            try
            {
                logger.LogInformation("Executing command {Command}", name);

                var result = await next();

                logger.LogInformation("Command {Command} processed successfully", name);

                return result;
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Command {Command} processing failed", name);

                throw;
            }
        }
    }
}
