using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MySite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProfile _profile;
        private readonly IPost _post;
        private readonly IFolower _folower;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ProfileController(UserManager<User> userManager, IProfile profile, IPost post, IFolower folower)
        {
            _userManager = userManager;
            _profile = profile;
            _post = post;
            _folower = folower;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            Profile MyProfile = _profile.Profiles.FirstOrDefault(p => p.UserID.Equals(user.Id));
            if (MyProfile == null)
            {
                _profile.SaveProfile(new Profile()
                {
                    UserID = user.Id
                });
                MyProfile = _profile.Profiles.FirstOrDefault(p => p.UserID.Equals(user.Id));
            }

            ProfileViewModel profileModel = new ProfileViewModel()
            {
                Profile = MyProfile,
                email = user.Email
                
                
            };
            return View(profileModel);
        }
        [HttpPost]
        public async Task<JsonResult> SubAjax([FromBody]AjaxPostViewModel model)
        {
            var CurrentUser = await GetCurrentUserAsync();
            var FolowerProfile = _post.Posts.FirstOrDefault(p => p.PostID == model.postID);
            if (CurrentUser != null && FolowerProfile != null)
            {
                if (!Url.IsLocalUrl(model.returnUrl))
                {
                    model.returnUrl = "/";
                }
                var folower = _folower.Folowers.Where(i => i.FolowerID.Equals(FolowerProfile.UserID));
                if (folower != null)
                {
                    if (CurrentUser.Id.Equals(FolowerProfile.UserID))
                    {
                        return Json("Exist");
                    }
                    foreach (var item in folower)
                    {
                        if (item.UserID.Equals(CurrentUser.Id))
                        {
                            _folower.DeleteFolower(item.ID);
                            return Json("Delete");
                        }
                    }
                    Folower modelFolower = new Folower()
                    {
                        FolowerID = FolowerProfile.UserID,
                        UserID = CurrentUser.Id
                      
                    };
                    _folower.AddFolower(modelFolower);
                    return Json("Add");
                }
            }
            if (CurrentUser == null)
            {
                return Json("NewUser");
            }
 
            return Json("Error");
        }
        //NOT Found fix
        [HttpPost]
        public async Task<IActionResult> AddFolower(int postID, string returnUrl)
        {

            var CurrentUser = await GetCurrentUserAsync();
            var FolowerProfile = _post.Posts.FirstOrDefault(p => p.PostID == postID);
            if (CurrentUser != null && FolowerProfile != null)
            {
                if (!Url.IsLocalUrl(returnUrl))
                {
                    returnUrl = "/";
                }
                var folower = _folower.Folowers.Where(i => i.FolowerID.Equals(FolowerProfile.UserID));
                if (folower != null)
                {
                    if (CurrentUser.Id.Equals(FolowerProfile.UserID))
                    {
                        return Redirect(returnUrl);
                    }
                    foreach (var item in folower)
                    {
                        if (item.UserID.Equals(CurrentUser.Id))
                        {
                            _folower.DeleteFolower(item.ID);
                            return Redirect(returnUrl);
                        }
                    }
                    Folower modelFolower = new Folower()
                    {
                        FolowerID = FolowerProfile.UserID,
                        UserID = CurrentUser.Id
                    };
                    _folower.AddFolower(modelFolower);
                }
            }
          
            return Redirect(returnUrl);
        }

    }
}
