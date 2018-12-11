using SampleApp.DirectPaymentCard.Consumer.RabbitMQ;

namespace SampleApp.DirectPaymentCard.Consumer
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
