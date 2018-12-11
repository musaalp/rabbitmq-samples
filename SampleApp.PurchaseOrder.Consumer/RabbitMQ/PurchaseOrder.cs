using System;

namespace SampleApp.PurchaseOrder.Consumer.RabbitMQ
{
    [Serializable]
    public class PurchaseOrder
    {
        public decimal Amount { get; set; }
        public int PurchaseOrderNumber { get; set; }
        public string CompanyName { get; set; }
        public int PaymentDayTerms { get; set; }
    }
}
