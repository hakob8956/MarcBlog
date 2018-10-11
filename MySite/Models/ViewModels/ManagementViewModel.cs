using System.Collections.Generic;

namespace MySite.Models.ViewModels
{
    public class ManagementViewModel
    {
        public ProfileViewModel ProfileViewModel { get; set; }
        public int Length { get; set; }
        public  IEnumerable<Profile> Profiles { get; set; }
        
    }
}
