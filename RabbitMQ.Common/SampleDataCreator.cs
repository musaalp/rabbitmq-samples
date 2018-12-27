using System;
using System.Collections.Generic;

namespace RabbitMQ.Common
{
    public class SampleDataCreator
    {
        public IEnumerable<Payment> CreatePayments(int sampleCount)
        {
            var customerList = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K' };

            var paymentList = new List<Payment>();

            for (int i = 1; i < sampleCount; i++)
            {
                var randomNumber = new Random().Next(1, customerList.Length - 1);

                var payment = new Payment
                {
                    Name = customerList[randomNumber].ToString(),
                    Amount = randomNumber * 7.3M,
                    CardNumber = i.ToString()
                };

                paymentList.Add(payment);
            }

            return paymentList;
        }

        public IEnumerable<PurchaseOrder> CreatePurchaseOrder(int sampleCount)
        {
            var companyList = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K' };

            var purchaseOrderList = new List<PurchaseOrder>();

            for (int i = 1; i < sampleCount; i++)
            {
                var randomNumber = new Random().Next(1, companyList.Length - 1);

                var payment = new PurchaseOrder
                {
                    CompanyName = companyList[randomNumber].ToString(),
                    Amount = randomNumber * 12.7M,
                    PaymentDayTerms = randomNumber * 5,
                    PurchaseOrderNumber = i
                };

                purchaseOrderList.Add(payment);
            }

            return purchaseOrderList;
        }
    }
}
