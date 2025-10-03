using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Models
{
    public class Attachment
    {

        [Key]
        public int Id { get; set; } 
        public string FileName { get; set; } 
        public string FilePath { get; set; } 
        public int UploadId { get; set; } 

        [ForeignKey("UploadId")] 
        public Upload Upload { get; set; } 

    }

}
