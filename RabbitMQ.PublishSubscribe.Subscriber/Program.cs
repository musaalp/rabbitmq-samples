using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.PublishSubscribe.Subscriber
{
    class Program
    {
        private const string ExchangeName = "PublishSubscribe_Exchange";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new QueueingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            DeclareAndBindQueueToExchange(channel, queueName);

            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,26}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("---------------------------------------------------------------------------------");

            while (true)
            {
                var ea = consumer.Queue.Dequeue();
                var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                
                Console.WriteLine("Payment message received : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
            }
        }

        private static void DeclareAndBindQueueToExchange(IModel channel, string queueName)
        {
            channel.ExchangeDeclare(ExchangeName, "fanout");            
            channel.QueueBind(queueName, ExchangeName, "");
        }
    }
}
