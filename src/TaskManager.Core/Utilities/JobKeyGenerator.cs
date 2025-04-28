namespace TaskManager.Core.Utilities;

public static class JobKeyGenerator
{
    public static string GenerateProjectNotificationKey(Guid projectId)
    {
        return $"ProjectNotification-{projectId}";
    }
}
