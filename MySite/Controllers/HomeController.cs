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

        public IActionResult Index(string category, int page = 1, string title = null) => View(new PostViewModel
        {
            Posts = _postRepository.Posts
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
                TotalItems = category == null || category == "All" ? //TODO FIX CATEGORY TEST title
                        _postRepository.Posts.Count() :
                        _postRepository.Posts.Where(e =>
                            e.Category == category).Count()
            },
            CurrentCategory = category,
            Categories = _postRepository.Posts.Select(x => x.Category).
                       Distinct().OrderBy(x => x)

        });

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
                if (FolowerProfile!=null && userId.Equals(FolowerProfile.UserID))
                {
                    _isSubscribe = true;
                }
                else if(ProfileFolowers != null)
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
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Test() => View();

    }
}
