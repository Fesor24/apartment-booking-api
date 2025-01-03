using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;
internal sealed class ProcessOutboxMessagesJobsSetup(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string jobName = nameof(ProcessOutboxMessagesJob);

        options
            .AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure => 
                   configure.ForJob(jobName)
                   .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(outboxOptions.Value.IntervalInSeconds)
                        .RepeatForever()));
    }
}
