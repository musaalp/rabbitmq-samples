using RabbitMQ.Client;
using RabbitMQ.Common;
using System;

namespace RabbitMQ.WorkerQueue.Consumer
{
    class Program
    {
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static IModel _model;

        private const string QueueName = "WorkerQueue_Queue";

        static void Main(string[] args)
        {
            CreateQueue();

            Receive();
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
            _model.BasicQos(0, 1, false);
        }

        public static void Receive()
        {
            var consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(QueueName, false, consumer);

            while (true)
            {
                var ea = consumer.Queue.Dequeue();
                var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                _model.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine($" Payment message received : {message.CardNumber} | {message.Amount} | {message.Name}");
            }
        }

        //private static void Receive()
        //{
        //    var consumer = new EventingBasicConsumer(_model);

        //    var strMessage = _model.BasicConsume(QueueName, true, consumer);

        //    consumer.Received += Consumer_Received;
        //}

        //private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        //{
        //    //todo: after receive second message the model is closing
        //    var message = e.Body.DeSerialize(typeof(Payment)) as Payment;
        //    _model.BasicAck(e.DeliveryTag, false);

        //    Console.WriteLine($" Message Received : {message.CardNumber} | {message.Amount} | {message.Name}");
        //}
    }
}
