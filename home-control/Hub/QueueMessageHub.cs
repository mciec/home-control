using home_control.Services;
using Microsoft.AspNetCore.Authorization;
using SignalR = Microsoft.AspNetCore.SignalR;
namespace home_control.Hub;

[Authorize]
public class QueueMessageHub : SignalR.Hub<IQueueMessageHub>
{
    private const string _queueMessageReceived = "QueueMessageReceived";
    private readonly QueueService _queueService;

    public QueueMessageHub(QueueService queueService)
    {
        _queueService = queueService;
    }

    public async Task SendMessageToQueue(string message)
    {
        await _queueService.SendAsync(message, CancellationToken.None);

        var x = Clients.All;
    }

    public async Task SendMessageToClients(string message)
    {
        try
        {
            await Clients.All.QueueMessageReceived(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}

