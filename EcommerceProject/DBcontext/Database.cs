using EcommerceProject.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.DBcontext
{
    public class Database :DbContext
    {

       public Database(DbContextOptions options) :base(options)
        {
        }

        public DbSet<SellerLogin> Seller { get; set; }   

        public DbSet<Upload> UploadTable { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        //public DbSet<Cart> Cart { get; set; }
        public DbSet<Cart> Carts { get; set; }



        public DbSet<CustomerLogin> Customers { get; set; }

        //public DbSet<CustomerLoginDTO> CustomerLoginDTO { get; set; } = default!;
        //public DbSet<EcommerceProject.Models.VerifyOtpDTO> VerifyOtpDTO { get; set; } = default!;

        //public DbSet<VerifyOtpDTO> VerifyOtpDTO { get; set; } = default!;
        public DbSet<Order> Orders { get; set; }

        public DbSet<Wishlist> Wishlist { get; set; }

    }
}
