namespace Discount.Core.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
         public string Description { get; set; }
        public int Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}