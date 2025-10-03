using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Views.Dto
{
    public class VerifyOtpDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "OTP must be 4 digits")]
        public string OtpCode { get; set; }

    }
}
