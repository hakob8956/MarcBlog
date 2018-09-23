using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models
{
    public interface IPost
    {
        IEnumerable<Post> Posts { get; }
        void SaveProduct(Post post);
        Post DeletePost(int postID);
    }
}
