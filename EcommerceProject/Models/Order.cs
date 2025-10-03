using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Upload Product { get; set; }

        public int Quantity { get; set; }
        public decimal Total { get; set; }


        public DateTime OrderDate { get; set; }
        public string Status { get; set; }  // Pending, Shipped, Delivered

        public int? SellerId { get; set; }
        [ForeignKey("SellerId")]
        public SellerLogin Seller { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerLogin Customer { get; set; }
    }
}
