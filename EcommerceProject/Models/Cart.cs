using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Foreign key for product
        public int UploadId { get; set; }

        // Logged-in user identifier (email/username)
        public string UserId { get; set; }

        // Quantity support
        public int Quantity { get; set; }

        [ForeignKey("UploadId")]
        public Upload Upload { get; set; } // Product details
    }
}
