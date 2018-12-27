using RabbitMQ.Client;
using RabbitMQ.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RabbitMQ.DirectRouting.Publisher
{
    class Program
    {
        private const string ExchangeName = "DirectRouting_Exchange";
        private const string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";

        static void Main(string[] args)
        {
            var connectionFactory = ConnectionFactoryProvider.Get();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var dataCreator = new SampleDataCreator();
            var payments = dataCreator.CreatePayments(100);
            var purchaseOrders = dataCreator.CreatePurchaseOrder(100);

            CreateConnection(channel);

            var paymentsQueue = new Queue<Payment>(payments);
            var purchaseOrdersQueue = new Queue<PurchaseOrder>(purchaseOrders);

            var onOff = true;

            var count = payments.Count() + purchaseOrders.Count();

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|", "Description", "Name", "Amount", "Number", "Other"));
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");

            for (int i = 0; i < count; i++)
            {
                if (onOff)
                {
                    var payment = paymentsQueue.Dequeue();
                    SendMessage(payment.Serialize(), "CardPayment", channel);
                    Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|", "Payment message sent: ", payment.Name, payment.Amount, payment.CardNumber));
                    onOff = false;
                }
                else
                {
                    var purchaseOrder = purchaseOrdersQueue.Dequeue();
                    SendMessage(purchaseOrder.Serialize(), "PurchaseOrder", channel);
                    Console.WriteLine(string.Format("|{0,25}|{1,20}|{2,20}|{3,20}|{4,20}|", "Purchase order sent: ", purchaseOrder.CompanyName, purchaseOrder.Amount, purchaseOrder.PurchaseOrderNumber, purchaseOrder.PaymentDayTerms));
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
                    onOff = true;
                }
            }
        }

        private static void CreateConnection(IModel channel)
        {
            channel.ExchangeDeclare(ExchangeName, "direct");

            channel.QueueDeclare(CardPaymentQueueName, true, false, false, null);
            channel.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);

            channel.QueueBind(CardPaymentQueueName, ExchangeName, "CardPayment");
            channel.QueueBind(PurchaseOrderQueueName, ExchangeName, "PurchaseOrder");
        }

        private static void SendMessage(byte[] message, string routingKey, IModel channel)
        {
            Thread.Sleep(500);
            channel.BasicPublish(ExchangeName, routingKey, null, message);
        }
    }
}
