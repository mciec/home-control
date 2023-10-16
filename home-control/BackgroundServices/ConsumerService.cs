using Confluent.Kafka;
using home_control.Hub;
using Microsoft.AspNetCore.SignalR;

namespace home_control.BackgroundServices;


public class ConsumerService : BackgroundService
{
    private readonly string _configFilePath;
    private readonly IHubContext<QueueMessageHub, IQueueMessageHub> _queueMessageHubContext;

    public ConsumerService(string configFilePath, IHubContext<QueueMessageHub, IQueueMessageHub> queueMessageHubContext)
    {
        _configFilePath = configFilePath;
        _queueMessageHubContext = queueMessageHubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(_configFilePath)
            .Build();

        const string topic = "topic-1";

        configuration["group.id"] = "kafka-dotnet-getting-started";
        configuration["auto.offset.reset"] = "earliest";

        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();

        consumer.Subscribe(topic);

        try
        {
            while (true)
            {
                var cr = consumer.Consume(stoppingToken);
                Console.WriteLine($"Consumed event from topic {topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");

                await _queueMessageHubContext.Clients.All.QueueMessageReceived(cr.Message.Value);
            }
        }
        finally
        {
            consumer.Close();
        }


    }
}

