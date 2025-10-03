namespace EcommerceProject.Views.Dto
{
    public class BuyNowDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }


        // Order details
        public int Quantity { get; set; } = 1;
        public string CustomerName { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string PostalCode { get; set; }

        public decimal Total => Quantity * Price;
    }

}
