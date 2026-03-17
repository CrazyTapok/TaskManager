using System.Linq.Expressions;
namespace TaskManager.Core.Interfaces.Services.Scheduling;

public interface IJobScheduler
{
    void ScheduleJob(string jobId, Expression<Action> methodCall, string cronExpression);
}
