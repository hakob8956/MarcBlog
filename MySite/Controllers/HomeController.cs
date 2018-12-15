using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MySite.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private IPost _postRepository;
        private IFolower _folower;
        private IProfile _profile;
        public int PageSize = 4;

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public HomeController(IPost postRepository, IFolower folower, UserManager<User> userManager, IProfile profile)
        {
            _postRepository = postRepository;
            _folower = folower;
            _userManager = userManager;
            _profile = profile;
        }
        public IEnumerable<Post> SortPosts(IEnumerable<Post> posts, string selectedSort)
        {
            switch (selectedSort)
            {
                case "New":
                    posts = _postRepository.Posts.OrderByDescending(l => l.DateTime);
                    break;
                case "Old":
                    posts = _postRepository.Posts.OrderBy(l => l.DateTime);
                    break;
                default:
                    posts = _postRepository.Posts.OrderByDescending(l => l.PostID);
                    break;
            }
            return posts;
        }
        public IActionResult Index(string category, string sortSelected, int page = 1, string title = null, int requestAjax = 0)
        {

            var posts = SortPosts(_postRepository.Posts, sortSelected);//Default give order?postID

            int totalItems = 1;
            if (title != null)
            {
                totalItems = _postRepository.Posts
                        .Where(p => p.Allow == 1)
                        .Where(p => category == null || category.Equals("All") || p.Category.Equals(category))
                        .Where(p => title == null ||
                        p.Title.ToLower().Replace(" ", string.Empty)
                        .Contains(title.ToLower().Replace(" ", string.Empty)))
                        .OrderBy(p => p.PostID).Count();
            }///check page 
            else if (category == null || category.Equals("All"))
            {
                totalItems = _postRepository.Posts.Where(p => p.Allow == 1).Count();
            }
            else
            {
                totalItems = _postRepository.Posts.Where(p => p.Allow == 1)
                         .Where(e => e.Category == category).Count();
            }
            var model = new PostViewModel
            {
                Posts = posts
                        .Where(p => p.Allow == 1)
                        .Where(p => category == null || category.Equals("All") || p.Category.Equals(category))
                        .Where(p => title == null ||
                        p.Title.ToLower().Replace(" ", string.Empty)
                        .Contains(title.ToLower()
                        .Replace(" ", string.Empty)))
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = totalItems
                },

                Categories = _postRepository.Posts.Where(p => p.Allow == 1)
                        .Select(x => x.Category)
                        .Distinct().OrderBy(x => x),
                CurrentCategory = category,
                CurrentSearchTitle = title

            };
            if (requestAjax == 1)
            {
                return PartialView("ShowBlocksPartial", model);
            }
            return View(model);

        }



        public async Task<IActionResult> Post(int postID, string returnUrl)
        {
            var Post = _postRepository?.Posts?.FirstOrDefault(p => p.PostID == postID);
            var PostUser = _userManager?.Users?.FirstOrDefault(p => p.Id.Equals(Post.UserID));
            var currentUser = await GetCurrentUserAsync();
            var userId = currentUser?.Id;
            var FolowerProfile = _profile?.Profiles?.FirstOrDefault(p => p?.UserID == PostUser?.Id);
            var ProfileFolowers = _folower?.Folowers?.Where(i => i.FolowerID.Equals(FolowerProfile?.UserID));
            bool _isSubscribe = false;
            if (currentUser != null)
            {
                if (FolowerProfile != null && userId.Equals(FolowerProfile.UserID))
                {
                    _isSubscribe = true;
                }
                else if (ProfileFolowers != null)
                {
                    foreach (var item in ProfileFolowers)
                    {
                        if (item.UserID.Equals(userId))
                        {
                            _isSubscribe = true;
                            break;
                        }
                    }
                }
            }

            if (Post != null && PostUser != null)
            {
                return View(new PostContentViewModel()
                {
                    Post = Post,
                    ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/",
                    PostFavorites = _folower.Folowers.Count(p => p.FolowerID.Equals(PostUser.Id)),
                    isSubscribe = _isSubscribe
                });
            }
            else
            {
                return BadRequest();
            }
        }
        public FileContentResult GetAvatar(int profileID)
        {
            Profile profile = _profile.Profiles.FirstOrDefault(p => p.ProfileID == profileID);
            if (profile != null && profile.ImageData != null && profile.ImageMimeType != null)
            {
                return File(profile.ImageData, profile.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
        public FileContentResult GetImage(int postID)
        {
            Post product = _postRepository.Posts.FirstOrDefault(p => p.PostID == postID);
            if (product != null && product.ImageData != null && product.ImageMimeType != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
        [Authorize]
        public IActionResult AddPost()
        {
            return View(new Post());
        }

        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Test() => View();


    }
}
