namespace home_control.Hub;

public interface IQueueMessageHub
{
    Task QueueMessageReceived(string message);
}
