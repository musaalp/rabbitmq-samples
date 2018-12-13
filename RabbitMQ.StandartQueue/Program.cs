using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System;
using System.Collections.Generic;


namespace RabbitMQ.StandartQueue
{
    class Program
    {
        private const string QueueName = "StandartQueue_Queue";

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection().AddTransient<ConnectionFactory>();

            var connectionFactory = serviceCollection.BuildServiceProvider().GetService<ConnectionFactory>();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var payments = CreatePayments(50);

            CreateQueue(channel);

            foreach (var payment in payments)
            {
                SendMessage(payment, channel);
            }

            Console.WriteLine("-----------------------------");

            Receive(channel);

            Console.ReadLine();
        }

        private static IEnumerable<Payment> CreatePayments(int sampleCount)
        {
            var paymentList = new List<Payment>();

            for (int i = 1; i < sampleCount; i++)
            {
                var payment = new Payment
                {
                    Amount = new Random().Next(1, 10) * i,
                    CardNumber = i.ToString(),
                    Name = $"Customer Name {i}"
                };

                paymentList.Add(payment);
            }

            return paymentList;
        }

        private static void CreateQueue(IModel channel)
        {
            channel.QueueDeclare(QueueName, true, false, false, null);
        }

        private static void SendMessage(Payment message, IModel channel)
        {
            channel.BasicPublish("", QueueName, null, message.Serialize());
            Console.WriteLine($"Payment message sent : {message.CardNumber} | {message.Amount} | {message.Name}");
        }

        private static void Receive(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            var strMessage = channel.BasicConsume(QueueName, true, consumer);

            consumer.Received += Consumer_Received;
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = e.Body.DeSerialize(typeof(Payment)) as Payment;

            Console.WriteLine($" Message Received : {message.CardNumber} | {message.Amount} | {message.Name}");
        }
    }
}
