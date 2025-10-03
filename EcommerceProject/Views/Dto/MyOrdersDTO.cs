namespace EcommerceProject.Views.Dto
{
    public class MyOrdersDTO
    {
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
