using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MySite.Models;
using MySite.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
namespace MySite.Infrastructure
{
    [HtmlTargetElement("showNotificationCount")]
    public class NotificationNewPosts : TagHelper
    {

        private IUrlHelperFactory urlHelperFactory;
        private IPost post;

        public NotificationNewPosts(IUrlHelperFactory helperFactory, IPost _post)
        {
            urlHelperFactory = helperFactory;
            post = _post;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

            output.Content.AppendHtml(post.Posts.Where(p => p.Allow == 0).Count().ToString());
        }
    }
}
