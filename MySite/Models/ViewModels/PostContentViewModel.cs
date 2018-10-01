
namespace MySite.Models.ViewModels
{
    public class PostContentViewModel
    {
        public Post Post { get; set; }
        public string ReturnUrl { get; set; }
        public int PostFavorites { get; set; }
        public bool isSubscribe { get; set; }


    }
}
