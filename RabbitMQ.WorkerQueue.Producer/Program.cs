using RabbitMQ.Client;
using RabbitMQ.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RabbitMQ.WorkerQueue.Producer
{
    class Program
    {
        private const string QueueName = "WorkerQueue_Queue";

        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            var payments = CreatePayments(5154);

            CreateQueue();

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

        private static void CreateQueue()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _connectionFactory.CreateConnection();

            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);
        }

        private static void SendMessage(Payment message)
        {
            _model.BasicPublish("", QueueName, null, message.Serialize());
            Console.WriteLine($"Payment message sent : {message.CardNumber} | {message.Amount} | {message.Name}");
        }
    }
}
