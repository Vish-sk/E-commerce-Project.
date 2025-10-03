using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class Upload
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        //public double Price { get; set; }
        public decimal Price { get; set; }   


        public int Stock { get; set; }   // NEW → Inventory tracking
        public bool IsActive { get; set; }  // NEW → Product Active/Inactive

        public int ? SellerId { get; set; }   // NEW → Product belongs to Seller
        public SellerLogin Seller { get; set; }


        public ICollection<Attachment> Attachments { get; set; }


    }
}

