namespace EcommerceProject.Views.Dto
{
    public class OrderDetailsDTO
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }          // For clickable image
        public DateTime OrderDate { get; set; }

        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string SellerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
