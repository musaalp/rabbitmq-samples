using RabbitMQ.Client;
using RabbitMQ.Common;
using System;
using System.Threading;

namespace RabbitMQ.WorkerQueue.Producer
{
    class Program
    {
        private const string QueueName = "WorkerQueue_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var payments = new SampleDataCreator().CreatePayments(200);

            CreateQueue(channel);

            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,22}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("-----------------------------------------------------------------------------");

            foreach (var payment in payments)
            {
                SendMessage(payment, channel);
            }
        }

        private static void CreateQueue(IModel channel)
        {
            channel.QueueDeclare(QueueName, true, false, false, null);
        }

        private static void SendMessage(Payment message, IModel channel)
        {
            Thread.Sleep(500);
            channel.BasicPublish("", QueueName, null, message.Serialize());
            Console.WriteLine("Payment message sent : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
        }
    }
}
