using System.ComponentModel.DataAnnotations;

namespace MySite.Models
{
    public class Profile
    {
        [Key]
        public int ProfileID { get; set; }
        public string UserID { get; set; }
        [MaxLength(10,ErrorMessage ="Max length 10")]
        [MinLength(4, ErrorMessage = "Min length 4")]
        
        public string FirstName { get; set; }
        [MaxLength(15, ErrorMessage = "Max length 15")]
        [MinLength(5, ErrorMessage = "Min length 5")]
        public string LastName { get; set; }
        [MaxLength(120, ErrorMessage = "Max length 120")]
        public string Description { get; set; }
        public int Folowers { get; set; }
        public int Folowing { get; set; }
        public int Viewers { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        

    }
}
