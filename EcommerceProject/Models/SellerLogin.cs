using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace EcommerceProject.Models
{
    public class SellerLogin
    {
        public int id { get; set; }
        public string Login { get; set; }

        public  string  Password { get; set; }

        public string Name { get; set; }

        public string GSTNO { get; set; }

        [Phone]
        public string MobileNo { get; set; }

        //public string? OtpCode { get; set; }
        //public DateTime? OtpGeneratedAt { get; set; }


        // Navigation
        public ICollection<Upload> Products { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
