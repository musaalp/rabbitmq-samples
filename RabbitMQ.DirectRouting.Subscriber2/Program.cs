using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.DirectRouting.Subscriber2
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";

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
                    channel.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);
                    channel.QueueBind(PurchaseOrderQueueName, ExchangeName, "PurchaseOrder");
                    channel.BasicQos(0, 1, false);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(PurchaseOrderQueueName, false, consumer);

                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();
                        var message = (PurchaseOrder)ea.Body.DeSerialize(typeof(PurchaseOrder));
                        var routingKey = ea.RoutingKey;
                        channel.BasicAck(ea.DeliveryTag, false);

                        Console.WriteLine("-- Purchase Order - Routing Key <{0}> : {1}, £{2}, {3}, {4}", routingKey, message.CompanyName, message.Amount, message.PaymentDayTerms, message.PurchaseOrderNumber);
                    }
                }
            }
        }
    }
}
