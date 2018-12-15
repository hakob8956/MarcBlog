using System;
using System.Collections.Generic;
using System.Linq;


namespace MySite.Models
{
    public class EFProfileRepository :IProfile
    {
        private ApplicationDbContext context;

        public EFProfileRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Profile> Profiles => context.Profiles;

        public void SaveProfile(Profile profile)
        {
            if (profile.ProfileID == 0)
            {
                context.Profiles.Add(profile);
            }
            else
            {
                //context.Profiles.AddRange(profile);
                Profile dbEntry = context.Profiles
                    .FirstOrDefault(p => p.ProfileID == profile.ProfileID);

                if (dbEntry != null)
                {
                    dbEntry.UserID = profile.UserID;
                    dbEntry.FirstName = profile.FirstName;
                    dbEntry.LastName = profile.LastName;
                    dbEntry.Description = profile.Description;
                    dbEntry.Folowers = profile.Folowers;
                    dbEntry.Viewers = profile.Viewers;
                    dbEntry.ImageData = profile.ImageData;
                    dbEntry.ImageMimeType = profile.ImageMimeType;
                }
            }
            context.SaveChanges();
        }

        public Profile DeleteProfile(int profileID)
        {
            Profile dbEntry = context.Profiles
                .FirstOrDefault(p => p.ProfileID == profileID);
            if (dbEntry != null)
            {
                context.Profiles.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

     
    }
}
