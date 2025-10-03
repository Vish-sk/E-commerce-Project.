// Models/CustomerLogin.cs
using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class CustomerLogin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? OtpCode { get; set; }
        public DateTime? OtpGeneratedAt { get; set; }

        [Required]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}