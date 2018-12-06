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

        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            var payments = CreatePayments(5154);

            CreateConnection();

            foreach (var payment in payments)
            {
                Thread.Sleep(1000);
                SendMessage(payment);
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

        private static void CreateConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _connectionFactory.CreateConnection();

            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeName, "fanout", false);
        }

        private static void SendMessage(Payment message)
        {
            _model.BasicPublish(ExchangeName, "", null, message.Serialize());
            Console.WriteLine($"Payment message sent : {message.CardNumber} | {message.Amount} | {message.Name}");
        }
    }
}
