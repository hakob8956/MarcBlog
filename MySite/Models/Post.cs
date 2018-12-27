using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MySite.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        public string UserID { get; set; }

        public int ProfileID { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter a description")]
        [MaxLength(250, ErrorMessage = "Max enter a 250 characters")]
        [MinLength(20, ErrorMessage = "Min enter a 20 characters")]

        public string Description { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Please enter a text")]
        [MinLength(50, ErrorMessage = "Min enter a 20 characters")]
        [MaxLength(30000, ErrorMessage = "Max enter a 30000  characters")]
        public string Text { get; set; }

        public string Author { get; set; }



        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public DateTime DateTime { get; set; }

        public byte Allow { get; set; } = 0;//It's 1 or 0 -->1 allow post(show) 0 -- none --> -1 delete


    }
}
