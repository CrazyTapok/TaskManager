namespace TaskManager.Core.Infrastructure;

public class NotFoundException : Exception
{
    public string Property { get; set; }
    public NotFoundException(string message, string prop) : base(message)
    {
        Property = prop;
    }
}
