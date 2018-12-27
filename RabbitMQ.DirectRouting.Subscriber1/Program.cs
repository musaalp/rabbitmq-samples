using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.DirectRouting.Subscriber1
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new QueueingBasicConsumer(channel);

            DeclareAndBindQueueToExchange(channel);

            channel.BasicConsume(CardPaymentQueueName, false, consumer);

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|", "Description", "Routing Key", "Company Name", "Amount", "Card Number"));
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");

            while (true)
            {
                var ea = consumer.Queue.Dequeue();
                var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                var routingKey = ea.RoutingKey;
                channel.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|", "Payment received", routingKey, message.Name, message.Amount, message.CardNumber));
            }
        }

        private static void DeclareAndBindQueueToExchange(IModel channel)
        {
            channel.ExchangeDeclare(ExchangeName, "direct");
            channel.QueueDeclare(CardPaymentQueueName, true, false, false, null);
            channel.QueueBind(CardPaymentQueueName, ExchangeName, "CardPayment");
            channel.BasicQos(0, 1, false);
        }
    }
}
