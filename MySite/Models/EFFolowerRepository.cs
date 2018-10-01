using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models
{
    public class EFFolowerRepository : IFolower
    {
        private ApplicationDbContext context;

        public EFFolowerRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Folower> Folowers => context.Folowers.ToList();

        public void AddFolower(Folower folower)
        {
            if (folower.ID == 0)
            {
                context.Folowers.Add(folower);
            }
            else
            {
                Folower dbEntry = context.Folowers
                    .FirstOrDefault(p => p.ID == folower.ID);
                if (dbEntry != null)
                {
                    dbEntry.FolowerID = folower.FolowerID;
                    dbEntry.UserID = folower.UserID;
                }
            }
            context.SaveChanges();
        }

        public Folower DeleteFolower(int folowerID)
        {
            Folower dbEntry = context.Folowers
                .FirstOrDefault(p => p.ID == folowerID);
            if (dbEntry != null)
            {
                context.Folowers.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

       
    }
}
