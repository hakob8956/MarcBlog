using System;
using System.Collections.Generic;
using System.Linq;


namespace MySite.Models
{
    public class EFProfileRepository 
    {
        private ApplicationDbContext context;

        public EFProfileRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Profile> Profiles => context.Profiles;

        public void SaveProduct(Profile profile)
        {
            if (profile.ProfileID == 0)
            {
                context.Profiles.Add(profile);
            }
            else
            {
                Profile dbEntry = context.Profiles
                    .FirstOrDefault(p => p.ProfileID == profile.ProfileID);
                if (dbEntry != null)
                {
                    dbEntry.UserID = profile.UserID;
                    dbEntry.FirstName = profile.FirstName;
                    dbEntry.LastName = profile.LastName;
                    dbEntry.Folowers = profile.Folowers;
                    dbEntry.Viewers = profile.Viewers;
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
