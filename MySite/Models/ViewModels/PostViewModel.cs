using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models.ViewModels
{
    public class PostViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; } = "";
        public string CurrentSearchTitle { get; set; } = null;
        public IEnumerable<string> Categories { get; set; }
    }
}
