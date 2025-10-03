using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Views.Dto
{
    public class UploadDTO
    {
   
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }   // was double before


        [Required]
        [Display(Name = "Upload Image")]
        [NotMapped] // 👈 Add this
        public IFormFile ? ImageFile { get; set; } //this is for image in front end 

        public string ? ImagePath { get;set; } //this is for storing image filepath to the database 

        // NEW: quantity shown in cart
        public int Quantity { get; set; }

    }
}
