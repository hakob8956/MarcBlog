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
        private IPost postRepository;
        public int PageSize = 4;

        public HomeController(IPost postRepository)
        {
            this.postRepository = postRepository;
        }

        public IActionResult Index(string category, int page = 1, string title = null) => View(new PostViewModel
        {
            Posts = postRepository.Posts
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
                TotalItems = category == null || title == null ?
                        postRepository.Posts.Count() :
                        postRepository.Posts.Where(e =>
                            e.Category == category).Count()
            },
            CurrentCategory = category,
            Categories=postRepository.Posts.Select(x => x.Category).
                       Distinct().OrderBy(x => x)

        });

        public IActionResult Post(int postID)
        {
            var model = postRepository.Posts.FirstOrDefault(p => p.PostID == postID);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }
        public FileContentResult GetImage(int postID)
        {
            Post product = postRepository.Posts.FirstOrDefault(p => p.PostID == postID );
            if (product != null)
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
