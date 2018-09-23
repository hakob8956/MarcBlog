using System.ComponentModel.DataAnnotations;
namespace MySite.ViewModels
{
    public class CreateUserViewModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Range(1898, 2010, ErrorMessage = "Invalid year(Range 1898-2010)")]
        [Required]
        [Display(Name = "Year of birth")]
        public int Year { get; set; }
        
        public string Date { get; set; }
    }
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Range(1898, 2010, ErrorMessage = "Invalid year(Range 1898-2010)")]
        [Required]
        [Display(Name = "Year of birth")]
        public int Year { get; set; }
        public string Date { get; set; }
    }
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }
        public string Date { get; set; }

    }
}
