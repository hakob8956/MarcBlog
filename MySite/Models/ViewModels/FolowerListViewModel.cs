using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySite.Models.ViewModels
{
    public class FolowerListViewModel
    {
        public string FolowerID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public bool isSubscribe { get; set; }

    }
}
