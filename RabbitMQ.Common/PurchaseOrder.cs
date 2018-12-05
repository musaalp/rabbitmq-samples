using System;

namespace RabbitMQ.Common
{
    [Serializable]
    public class PurchaseOrder
    {
        public decimal Amount { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CompanyName { get; set; }
        public int PaymentDayTerms { get; set; }
    }
}
