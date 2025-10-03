using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

   
        public int? CurrentUserId { get; set; }


        [ForeignKey("ProductId")]
        public Upload Upload { get; set; }
    }
}
