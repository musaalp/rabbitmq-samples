using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.WorkerQueue.Consumer
{
    class Program
    {
        private const string QueueName = "WorkerQueue_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            CreateQueue(channel);

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,26}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("---------------------------------------------------------------------------------");

            ReceiveMessages(channel);
        }

        private static void CreateQueue(IModel channel)
        {
            channel.QueueDeclare(QueueName, true, false, false, null);
            //This tells RabbitMQ not to give more than one message to a worker at a time
            channel.BasicQos(0, 1, false);
        }

        public static void ReceiveMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (ch, eventArgs) =>
            {
                var message = (Payment)eventArgs.Body.DeSerialize(typeof(Payment));
                channel.BasicAck(eventArgs.DeliveryTag, false);

                Console.WriteLine("Payment message received : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
            };

            channel.BasicConsume(QueueName, false, consumer);
        }
    }
}
