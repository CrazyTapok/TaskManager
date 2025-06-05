using System.Linq.Expressions;
namespace TaskManager.Core.Interfaces.Services;

public interface IJobSchedulerWrapper
{
    void ScheduleJob(string jobId, Expression<Action> methodCall, string cronExpression);
}
