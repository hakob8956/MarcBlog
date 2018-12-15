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
        public int PageSize = 4;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ProfileController(UserManager<User> userManager, IProfile profile, IPost post, IFolower folower)
        {
            _userManager = userManager;
            _profile = profile;
            _post = post;
            _folower = folower;
        }
        private PostViewModel SetPost(IEnumerable<Post> posts, string category, int page = 1, string title = null)
        {
            PostViewModel model = new PostViewModel()
            {
                Posts = posts
                    .Where(p => category == null || category.Equals("All") || p.Category == category)
                    .Where(p => title == null ||
                     p.Title.ToLower().Replace(" ", string.Empty)
                    .Contains(title.ToLower()
                    .Replace(" ", string.Empty)))
                    .OrderBy(p => p.PostID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null || category == "All" ?
                        posts.Count() :
                        posts.Where(e =>
                            e.Category == category).Count()
                },
                CurrentCategory = category,
                Categories = posts.Select(x => x.Category).
                       Distinct().OrderBy(x => x)
            };
            if (model.Posts.Count(p => p.PostID == p.PostID) == 0)
            {
                model = null;
            }
            return model;
        }

        public async Task<IActionResult> Index(int profileID, string category, int page = 1, string title = null, int ajaxRequest = 0, int allowEdit = 0)
        {
            var user = await GetCurrentUserAsync();

            Profile profile = _profile.Profiles.FirstOrDefault(p => p.ProfileID == profileID);
            if (profile != null)
            {

                var posts = _post.Posts.Where(p => p.ProfileID == profile.ProfileID);
                PostViewModel postModel = SetPost(posts, category, page, title);
                var userPostProfile = _userManager.Users.FirstOrDefault(p => p.Id == profile.UserID);//Who create post
                bool Me = true ? user != null && user.Id == userPostProfile.Id : false;
                ProfileViewModel profileModel = new ProfileViewModel
                {
                    Profile = profile,
                    email = userPostProfile.Email,
                    MyPosts = postModel,
                    Me = Me,//if user exist and userId == profileID
                    AllowEdit = Me ? allowEdit : 0

                };
                if (ajaxRequest == 1)//FOR AJAX REQUEST
                {
                    return PartialView("ShowProfilePageHeader", profileModel);
                }
                return View(profileModel);

            }
            return NotFound();
        }

        //[HttpGet]
        //public IActionResult Edit()
        //{

        //    return RedirectToAction("Index", new { ajaxRequest = 0, allowEdit = 1,profileID= });
        //}
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
                profile.Description = model.Description;
                model.ProfileID = profile.ProfileID;



                _profile.SaveProfile(profile);

                ProfileViewModel profileModel = new ProfileViewModel()
                {
                    Profile = model,
                    email = CurrentUser.Email
                };
                return RedirectToAction("Index", new { profileID = profile.ProfileID });
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
                return RedirectToAction("Index", new { profileID = profileModel.Profile.ProfileID });
            }
        }
        [HttpPost]
        public async Task<JsonResult> SubAjax([FromBody]AjaxPostViewModel model)
        {
            var CurrentUser = await GetCurrentUserAsync();
            Post FolowerAccount = null;
            Profile FolowerProfile = null;
            if (model.postID == 0)
            {
                FolowerProfile = _profile.Profiles.FirstOrDefault(p => model.profileID == p.ProfileID);
            }
            else
            {
                FolowerAccount = _post.Posts.FirstOrDefault(p => p.PostID == model.postID);
                if (FolowerAccount != null)
                    FolowerProfile = _profile.Profiles.FirstOrDefault(p => FolowerAccount.UserID == p.UserID);
            }

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
                            FolowerProfile.Folowers--;

                            _folower.DeleteFolower(item.ID);
                            _profile.SaveProfile(FolowerProfile);
                            return Json("Delete");
                        }
                    }
                    Folower modelFolower = new Folower()
                    {
                        FolowerID = FolowerProfile.UserID,
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
