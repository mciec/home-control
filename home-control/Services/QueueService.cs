using Confluent.Kafka;

namespace home_control.Services;


public class QueueService : IDisposable
{
    private readonly string _configFilePath;
    private readonly IProducer<string, string> _producer;
    public QueueService(string configFilePath)
    {
        _configFilePath = configFilePath;
        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(_configFilePath)
            .Build();

        _producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();
    }

    public async Task SendAsync(string message, CancellationToken stoppingToken, string topic = "topic-1")
    {
        var queueMessage = new Message<string, string>() { Key = Guid.NewGuid().ToString(), Value = message };

        try
        {
            await _producer.ProduceAsync(topic, queueMessage);
            _producer.Flush();
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }


}

