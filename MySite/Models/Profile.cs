using System.ComponentModel.DataAnnotations;

namespace MySite.Models
{
    public class Profile
    {
        [Key]
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Folowers { get; set; }
        public int Viewers { get; set; }
    }
}
