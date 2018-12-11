using SampleApp.PaymentCard.Consumer.RabbitMQ;

namespace SampleApp.PaymentCard.Consumer
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
