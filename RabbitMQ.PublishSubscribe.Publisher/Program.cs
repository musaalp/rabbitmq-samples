using RabbitMQ.Client;
using RabbitMQ.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RabbitMQ.PublishSubscribe.Publisher
{
    class Program
    {
        private const string ExchangeName = "PublishSubscribe_Exchange";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, "fanout", false);

            var payments = CreatePayments(1000);

            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,22}|{1,15}|{2,15}|{3,20}|", "Description", "Card Number", "Amount", "Name"));
            Console.WriteLine("-----------------------------------------------------------------------------");

            foreach (var payment in payments)
            {
                Thread.Sleep(500);
                SendMessage(payment, channel);
            }
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

        private static void SendMessage(Payment message, IModel channel)
        {
            channel.BasicPublish(ExchangeName, "", null, message.Serialize());
            Console.WriteLine("Payment message sent : " + string.Format("|{0,15}|{1,15}|{2,20}|", message.CardNumber, message.Amount, message.Name));
        }
    }
}
