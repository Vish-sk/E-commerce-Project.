// DTOs/SellerDashboardDTO.cs
namespace EcommerceProject.DTOs
{
    public class SellerDashboardDTO
    {
        public string SellerName { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // Table Data
        public List<SellerProductDTO> Products { get; set; } = new List<SellerProductDTO>();
    }

    public class SellerProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public string StockStatus { get; set; } // Active/Inactive
        public decimal Amount { get; set; }
        public string OrderStatus { get; set; } // Pending, Packed, Shipped, Delivered
    }
}
