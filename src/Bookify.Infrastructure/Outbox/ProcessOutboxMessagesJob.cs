using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal class ProcessOutboxMessagesJob(
    IDateTimeProvider dateTimeProvider, 
    ISqlConnectionFactory sqlConnectionFactory,
    IPublisher publisher,
    OutboxOptions outboxOptions,
    ILogger<ProcessOutboxMessagesJob> logger) : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Beginning to process outbox messages");

        using var connection = sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessages(connection, transaction);

        foreach(var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content, 
                    JsonSerializerSettings)!;

                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, 
                    "Exception while processing outbox message {MessageId}", 
                    outboxMessage.Id);

                exception = ex;
            }

            await UpdateOutboxMessagesAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        logger.LogInformation("Completed processing outbox messages");

    }

    private async Task UpdateOutboxMessagesAsync(IDbConnection connection, 
        IDbTransaction transaction, 
        OutboxMessageResponse outboxMessage, 
        Exception? exception)
    {
        const string sql = @"
                UPDATE outbox_messages
                SET processed_on_utc = @ProcessedOnUtc,
                    error = @Error
                WHERE id = @Id
            ";

        await connection.ExecuteAsync(sql, new
        {
            outboxMessage.Id,
            ProcessedOnUtc = dateTimeProvider.UtcNow,
            Error = exception?.ToString()
        }, transaction:transaction);


    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessages(IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT id, content
            FROM outbox_messages
            WHERE processed_on_utc IS NULL
            ORDER BY occurred_on_utc
            LIMIT {outboxOptions.BatchSize}
            FOR UPDATE
            """;

        // for update will lock any rows that was queried until we commit the transaction...

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

        return outboxMessages.ToList();
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
