using Hangfire;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Services;

internal class JobSchedulerWrapper : IJobSchedulerWrapper
{
    private readonly IRecurringJobManager _recurringJobManager;

    public JobSchedulerWrapper(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void ScheduleJob(string jobId, Expression<Action> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }
}
