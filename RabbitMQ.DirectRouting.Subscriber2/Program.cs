using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.DirectRouting.Subscriber2
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new QueueingBasicConsumer(channel);

            channel.ExchangeDeclare(ExchangeName, "direct");
            channel.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);
            channel.QueueBind(PurchaseOrderQueueName, ExchangeName, "PurchaseOrder");
            channel.BasicQos(0, 1, false);
            DeclareAndBindQueueToExchange(channel);

            channel.BasicConsume(PurchaseOrderQueueName, false, consumer);

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|{5,20}|", "Description", "Routing Key", "Company Name", "Amount", "Day Terms", "Order Number"));
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");

            while (true)
            {
                var ea = consumer.Queue.Dequeue();
                var message = (PurchaseOrder)ea.Body.DeSerialize(typeof(PurchaseOrder));
                var routingKey = ea.RoutingKey;
                channel.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|{5,20}|", "Purchase Order received", routingKey, message.CompanyName, message.Amount, message.PaymentDayTerms, message.PurchaseOrderNumber));
            }
        }

        private static void DeclareAndBindQueueToExchange(IModel channel)
        {
            channel.ExchangeDeclare(ExchangeName, "direct");
            channel.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);
            channel.QueueBind(PurchaseOrderQueueName, ExchangeName, "PurchaseOrder");
            channel.BasicQos(0, 1, false);
        }
    }
}
