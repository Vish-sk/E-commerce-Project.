namespace EcommerceProject.Views.Dto
{
    public class WishlistDTO
    {
        public int Id { get; set; }            // wishlist row Id
        public int ProductId { get; set; }     // product linked
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }   // was double before

        public string? ImagePath { get; set; } // product image path
    }
}
