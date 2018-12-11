using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using SampleApp.WebAPI.Models;
using System;
using System.Text;

namespace SampleApp.WebAPI.RabbitMQ
{
    public class RabbitMQDirectClient
    {
        private IConnection _connection;
        private IModel _channel;
        private string _replyQueueName;
        private string _authCode;
        private string _corrId;
        private EventingBasicConsumer consumer;

        public void CreateConnection()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _replyQueueName = _channel.QueueDeclare("rpc_reply", true, false, false, null);

            consumer = new EventingBasicConsumer(_channel);


            //_consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(_replyQueueName, true, consumer);
        }

        public void Close()
        {
            _connection.Close();
        }

        public string MakePayment(CardPayment payment)
        {
            _corrId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = _corrId;

            _channel.BasicPublish("", "rpc_queue", props, payment.Serialize());

            consumer.Received += Consumer_Received;

            while (true)
            {
                consumer.Received += Consumer_Received;

                //var ea = _consumer.Queue.Dequeue();

                //if (ea.BasicProperties.CorrelationId != corrId) continue;

                //var authCode = Encoding.UTF8.GetString(ea.Body);
                //return authCode;
            }

            return _authCode;
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if (e.BasicProperties.CorrelationId == _corrId)
            {
                _authCode = Encoding.UTF8.GetString(e.Body);
            }
        }
    }
}
