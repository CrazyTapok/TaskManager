using Hangfire;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Services.Scheduling;

namespace TaskManager.Infrastructure.Services.Reporting;

internal class HangfireJobScheduler : IJobScheduler
{
    private readonly IRecurringJobManager _recurringJobManager;

    public HangfireJobScheduler(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void ScheduleJob(string jobId, Expression<Action> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }
}
