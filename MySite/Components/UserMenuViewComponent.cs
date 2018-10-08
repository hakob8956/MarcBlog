using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Components
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly IProfile _profile;
        static object locker = new object();
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private string UserID { get; set; } = "0";
        public UserMenuViewComponent(UserManager<User> userManager, IProfile profile)
        {
            _userManager = userManager;
            _profile = profile;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetCurrentUserAsync();
                UserID = user.Id;
            }
            return View(_profile.Profiles.FirstOrDefault(p => p.UserID == UserID));
        }

    }
}
