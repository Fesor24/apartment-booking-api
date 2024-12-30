using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors
{
    internal class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : 
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
        where TResponse: Result
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var name = request.GetType().Name;

            try
            {
                logger.LogInformation("Executing request {Request}", name);

                //reason why TResponse is of type Result is so we can capture the result in a variable
                var result = await next();

                if (result.IsSuccess)
                    logger.LogInformation("Request {Request} processed successfully", name);

                //else
                //    logger.LogError("Request {Request} processed with an {Error}", name, result.Error);
                // will serialize as a string, alternative we can use @Error and it will be serialized into a json string
                // allowing to filter based on the properties...

                // we could howver push the error into the log context
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true)) // true- to destructure the object so it is serialized
                    {
                        logger.LogError("Request {Request} processed with {Error}", name, result.Error);
                    }
                }


                return result;
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Request {Request} processing failed", name);

                throw;
            }
        }
    }
}
