using SampleApp.PurchaseOrder.Consumer.RabbitMQ;

namespace SampleApp.PurchaseOrder.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RabbitMQConsumer();
            client.CreateConnection();
            client.ProcessMessages();
            client.Close();
        }
    }
}
