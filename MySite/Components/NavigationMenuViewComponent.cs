using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MySite.Models;
namespace MySite.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IPost repository;

        public NavigationMenuViewComponent(IPost repository)
        {
            this.repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View
                (
                repository.Posts
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x)
                );
        }

    }
}