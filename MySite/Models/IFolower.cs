using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models
{
    public interface IFolower
    {
        IEnumerable<Folower> Folowers { get; }
        void AddFolower(Folower folower);
        Folower DeleteFolower(int folowerID);
    }
}
