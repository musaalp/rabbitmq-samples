using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System;


namespace RabbitMQ.StandartQueue
{
    class Program
    {
        private const string QueueName = "StandartQueue_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var payments = new SampleDataCreator().CreatePayments(20);

            CreateQueue(channel);

            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,22}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("-----------------------------------------------------------------------------");
            foreach (var payment in payments)
            {
                SendMessage(payment, channel);
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,26}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("---------------------------------------------------------------------------------");

            ReceiveMessages(channel);
        }

        private static void CreateQueue(IModel channel)
        {
            channel.QueueDeclare(QueueName, true, false, false, null);
        }

        private static void SendMessage(Payment message, IModel channel)
        {
            channel.BasicPublish("", QueueName, null, message.Serialize());
            Console.WriteLine("Payment message sent : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
        }

        private static void ReceiveMessages(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            var strMessage = channel.BasicConsume(QueueName, true, consumer);

            consumer.Received += Consumer_Received;
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = e.Body.DeSerialize(typeof(Payment)) as Payment;
            Console.WriteLine("Payment message received : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
        }
    }
}
