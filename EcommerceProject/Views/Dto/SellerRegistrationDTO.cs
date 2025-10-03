using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Views.Dto
{
    public class SellerRegistrationDTO
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string GSTNO { get; set;}

        public string MobileNumber { get; set;}

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set;}

        [Required(ErrorMessage = "password incorrect")]
        [Compare("Password", ErrorMessage = "password doesn't match")]
        public string ConfirmPassword { get; set;}

    }
}
