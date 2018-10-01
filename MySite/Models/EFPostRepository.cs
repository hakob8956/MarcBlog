using System;
using System.Collections.Generic;
using System.Linq;


namespace MySite.Models
{
    public class EFPostRepository:IPost
    {
        private ApplicationDbContext context;

        public EFPostRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Post> Posts => context.Posts;

        public void SaveProduct(Post post)
        {
            if (post.PostID == 0)
            {
                context.Posts.Add(post);
            }
            else
            {
                Post dbEntry = context.Posts
                    .FirstOrDefault(p => p.PostID == post.PostID);
                if (dbEntry != null)
                {
                    dbEntry.UserID = post.UserID;
                    dbEntry.Title = post.Title;
                    dbEntry.Description = post.Description;                   
                    dbEntry.Category = post.Category;
                    dbEntry.Text = post.Text;
                    dbEntry.Author = post.Author;
                    dbEntry.DateTime = post.DateTime;
                    dbEntry.ImageData = post.ImageData;
                    dbEntry.ImageMimeType = post.ImageMimeType;
                
                }
            }
            context.SaveChanges();
        }

        public Post DeletePost(int postID)
        {
            Post dbEntry = context.Posts
                .FirstOrDefault(p => p.PostID == postID);
            if (dbEntry != null)
            {
                context.Posts.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}
