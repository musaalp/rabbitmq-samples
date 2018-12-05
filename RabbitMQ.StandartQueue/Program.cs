using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System;
using System.Collections.Generic;

namespace RabbitMQ.StandartQueue
{
    class Program
    {
        private const string QueueName = "StandartQueue_ExampleQueue";

        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            var payments = CreatePayments(6);

            CreateQueue();

            foreach (var payment in payments)
            {
                SendMessage(payment);
            }

            Console.WriteLine();

            Receive();

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

        private static void Receive()
        {
            var consumer = new EventingBasicConsumer(_model);

            var strMessage = _model.BasicConsume(QueueName, true, consumer);

            consumer.Received += Consumer_Received;

            var messageCount = GetMessagesCount(_model, QueueName);
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = e.Body.DeSerialize(typeof(Payment)) as Payment;

            Console.WriteLine($" Message Received : {message.CardNumber} | {message.Amount} | {message.Name}");
        }

        private static uint GetMessagesCount(IModel channel, string queueName)
        {
            var count = channel.QueueDeclare(queueName, true, false, false, null).MessageCount;
            return count;
        }
    }
}
