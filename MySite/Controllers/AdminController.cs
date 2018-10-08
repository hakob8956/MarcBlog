using MySite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MySite.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        
        private IPost repository;
        public const int ImageMinimumBytes = 512;
        private readonly UserManager<User> _userManager;
        private readonly IProfile _profile;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AdminController(IPost repository, UserManager<User> userManager,IProfile profile)
        {
            _profile = profile;
            _userManager = userManager;
            this.repository = repository;

        }
        public ViewResult Index() => View(repository.Posts);
        public ViewResult Edit(int postID) =>
            View(repository.Posts.FirstOrDefault(p => p.PostID == postID));

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
        [HttpPost]
        public IActionResult DeleteAvatar(int profileID)
        {

            Profile profile = _profile.Profiles.FirstOrDefault(p => p.ProfileID == profileID);
            if (profile != null)
            {
                profile.ImageData = null;
                profile.ImageMimeType = null;
                _profile.SaveProfile(profile);
            }
            return View("Edit", "Profile");
        }
        [HttpPost]
        public IActionResult DeleteImage(int postID)
        {
            
            Post post = repository.Posts.FirstOrDefault(p => p.PostID == postID);
            if (post != null)
            {
                post.ImageData = null;
                post.ImageMimeType = null;
                repository.SaveProduct(post);
            }
            return View("Edit", postID);
        }
        [HttpPost]
        public  async Task<IActionResult> Edit(Post post, IFormFile image = null)
        {         
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    if (IsImage(image))
                    {
                        post.ImageMimeType = image.ContentType;
                        post.ImageData = new byte[image.Length];
                        image.OpenReadStream().Read(post.ImageData, 0, (int)image.Length);
                    }
                    else
                    {
                        TempData["error"] = $"It's not image";
                        return View(post);
                    }

                }
                var user = await GetCurrentUserAsync();
                post.UserID = user.Id;
                post.DateTime = DateTime.Now;
                repository.SaveProduct(post);
                TempData["message"] = $"{post.Title} has been saved";
                return View("Edit",post.PostID);
            }
            else
            {
                return View(post);
            }
        }

        public IActionResult Menu() => View();
        public ViewResult Create() => View("Edit", new Post());

        [HttpPost]
        public IActionResult Delete(int postID)
        {
            Post deletedPost = repository.DeletePost(postID);
            if (deletedPost != null)
                TempData["message"] = $"{ deletedPost.Title} was deleted";
            else
                TempData["message"] = $"{ deletedPost.Title} was'nt deleted";

            return RedirectToAction("Index");
        }
    }
}
