using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.DirectRouting.Subscriber1
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";

        private static ConnectionFactory _factory;
        private static IConnection _connection;

        static void Main(string[] args)
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.ExchangeDeclare(ExchangeName, "direct");
                    channel.QueueDeclare(CardPaymentQueueName, true, false, false, null);
                    channel.QueueBind(CardPaymentQueueName, ExchangeName, "CardPayment");
                    channel.BasicQos(0, 1, false);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(CardPaymentQueueName, false, consumer);

                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();
                        var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                        var routingKey = ea.RoutingKey;
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine("--- Payment - Routing Key <{0}> : {1} : {2}", routingKey, message.CardNumber, message.Amount);
                    }
                }
            }
        }
    }
}
