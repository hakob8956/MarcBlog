using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
namespace MySite.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
        public string Date { get; set; }
        
    }
}
