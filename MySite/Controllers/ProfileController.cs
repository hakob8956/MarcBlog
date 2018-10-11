using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MySite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProfile _profile;
        private readonly IPost _post;
        private readonly IFolower _folower;
        public const int ImageMinimumBytes = 512;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ProfileController(UserManager<User> userManager, IProfile profile, IPost post, IFolower folower)
        {
            _userManager = userManager;
            _profile = profile;
            _post = post;
            _folower = folower;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                Profile MyProfile = _profile.Profiles.FirstOrDefault(p => p.UserID.Equals(user.Id));

                ProfileViewModel profileModel = new ProfileViewModel()
                {
                    Profile = MyProfile,
                    email = user.Email

                };
                return View(profileModel);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                Profile MyProfile = _profile.Profiles.FirstOrDefault(p => p.UserID.Equals(user.Id));
                ProfileViewModel profileModel = new ProfileViewModel()
                {
                    Profile = MyProfile,
                    email = user.Email

                };
                return View(profileModel);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Profile model, IFormFile Image = null)
        {
            var CurrentUser = await GetCurrentUserAsync();
            if (CurrentUser != null && ModelState.IsValid)
            {
                var profile = _profile.Profiles.FirstOrDefault(p => p.UserID == CurrentUser.Id);

                if (Image != null)
                {

                    if (IsImage(Image))
                    {
                        profile.ImageMimeType = Image.ContentType;
                        profile.ImageData = new byte[Image.Length];
                        Image.OpenReadStream().Read(profile.ImageData, 0, (int)Image.Length);
                       

                    }
                    else
                    {
                        TempData["error"] = $"It's not image";
                    }

                }
                profile.FirstName = model.FirstName;
                profile.LastName = model.LastName;
                model.ProfileID = profile.ProfileID;
                

                _profile.SaveProfile(profile);

                ProfileViewModel profileModel = new ProfileViewModel()
                {
                    Profile = model,
                    email = CurrentUser.Email
                };
                return View(profileModel);
            }
            else if (CurrentUser == null)
            {
                return NotFound();
            }
            else
            {
                ProfileViewModel profileModel = new ProfileViewModel()
                {
                    Profile = model,
                    email = CurrentUser.Email
                };
                return View(profileModel
);
            }
        }
        [HttpPost]
        public async Task<JsonResult> SubAjax([FromBody]AjaxPostViewModel model)
        {
            var CurrentUser = await GetCurrentUserAsync();
            var FolowerAccount = _post.Posts.FirstOrDefault(p => p.PostID == model.postID);
            var FolowerProfile = _profile.Profiles.FirstOrDefault(p => FolowerAccount.UserID == p.UserID);
            if (CurrentUser != null && FolowerAccount != null)
            {
                if (!Url.IsLocalUrl(model.returnUrl))
                {
                    model.returnUrl = "/";
                }
                var folower = _folower.Folowers.Where(i => i.FolowerID.Equals(FolowerAccount.UserID));
                if (folower != null)
                {
                    if (CurrentUser.Id.Equals(FolowerAccount.UserID))
                    {
                        return Json("Exist");
                    }
                    foreach (var item in folower)
                    {
                        if (item.UserID.Equals(CurrentUser.Id))
                        {
                            FolowerProfile.Folowers--;

                            _folower.DeleteFolower(item.ID);
                            _profile.SaveProfile(FolowerProfile);
                            return Json("Delete");
                        }
                    }
                    Folower modelFolower = new Folower()
                    {
                        FolowerID = FolowerAccount.UserID,
                        UserID = CurrentUser.Id

                    };
                    FolowerProfile.Folowers++;
                    _folower.AddFolower(modelFolower);
                    _profile.SaveProfile(FolowerProfile);
                    return Json("Add");
                }
            }
            if (CurrentUser == null)
            {
                return Json("NewUser");
            }

            return Json("Error");
        }

        //[HttpGet]
        //public async Task<IActionResult> Management()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {

        //        Profile MyProfile = _profile.Profiles.FirstOrDefault(p => p.UserID.Equals(user.Id));
        //        ProfileViewModel profileModel = new ProfileViewModel()
        //        {
        //            Profile = MyProfile,
        //            email = user.Email

        //        };
        //        //ov iran subscribe tvel
        //        ManagementViewModel model = new ManagementViewModel()
        //        {
        //            ProfileViewModel = profileModel,
        //            Length = _folower.Folowers.Count(f => f.FolowerID == user.Id)

        //        }
        //        return View(profileModel);
        //    }
        //    return NotFound();
        //}
        public static bool IsImage(IFormFile postedFile)
        {

            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }

                if (postedFile.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.OpenReadStream().Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
