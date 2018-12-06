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

        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            var payments = CreatePayments(100);
            var purchaseOrders = CreatePurchaseOrder(100);

            CreateConnection();

            var paymentsQueue = new Queue<Payment>(payments);
            var purchaseOrdersQueue = new Queue<PurchaseOrder>(purchaseOrders);

            var onOff = true;

            var count = payments.Count() + purchaseOrders.Count();
            for (int i = 0; i < count; i++)
            {
                if (onOff)
                {
                    var payment = paymentsQueue.Dequeue();
                    SendMessage(payment.Serialize(), "CardPayment");
                    Console.WriteLine($"Card Payment Sent: {payment.CardNumber}, £{payment.Amount}");
                    onOff = false;
                }
                else
                {
                    var purchaseOrder = purchaseOrdersQueue.Dequeue();
                    SendMessage(purchaseOrder.Serialize(), "PurchaseOrder");
                    Console.WriteLine($"Purchase Order Sent: {purchaseOrder.PurchaseOrderNumber}, {purchaseOrder.CompanyName}, £{purchaseOrder.Amount}, {purchaseOrder.PaymentDayTerms}");
                    onOff = true;
                }
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

        private static IEnumerable<PurchaseOrder> CreatePurchaseOrder(int sampleCount)
        {
            var companyList = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K' };

            var purchaseOrderList = new List<PurchaseOrder>();

            for (int i = 1; i < sampleCount; i++)
            {
                var randomNumber = new Random().Next(1, companyList.Length - 1);

                var payment = new PurchaseOrder
                {
                    Amount = randomNumber * 12.5M,
                    CompanyName = companyList[randomNumber].ToString(),
                    PaymentDayTerms = randomNumber * 5,
                    PurchaseOrderNumber = i
                };

                purchaseOrderList.Add(payment);
            }

            return purchaseOrderList;
        }

        private static void CreateConnection()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeName, "direct");
            _model.QueueDeclare(CardPaymentQueueName, true, false, false, null);
            _model.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);

            _model.QueueBind(CardPaymentQueueName, ExchangeName, "CardPayment");
            _model.QueueBind(PurchaseOrderQueueName, ExchangeName, "PurchaseOrder");
        }

        private static void SendMessage(byte[] message, string routingKey)
        {
            Thread.Sleep(1000);
            _model.BasicPublish(ExchangeName, routingKey, null, message);
        }
    }
}
