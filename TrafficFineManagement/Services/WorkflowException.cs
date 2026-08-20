namespace TrafficFineManagement.Services;

public class WorkflowException : InvalidOperationException
{
    public WorkflowException(string message)
        : base(message)
    {
    }
}
