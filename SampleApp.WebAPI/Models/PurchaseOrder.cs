namespace SampleApp.WebAPI.Models
{
    public class PurchaseOrder : BaseModel
    {
        public string CompanyName { get; set; }
        public int PaymentDayTerms { get; set; }
    }
}
