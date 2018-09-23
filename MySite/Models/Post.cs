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

        public int UserID { get; set; }


        [Required(ErrorMessage = "Please enter a product name")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Please enter a text")]
        public string Text { get; set; }
        
        public string Author { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public DateTime DateTime { get; set; }

    }
}
