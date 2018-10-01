using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models
{
    public interface IProfile
    {
        IEnumerable<Profile> Profiles { get; }
        void SaveProfile(Profile profile);
         Profile DeleteProfile(int profileID);
    }
}
