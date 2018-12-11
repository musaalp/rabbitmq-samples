using SampleApp.AccountAudit.Consumer.RabbitMQ;

namespace SampleApp.AccountAudit.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RabbitMQConsumer();
            client.CreateConnection();
            client.ProcessMessages();
        }
    }
}
