﻿using System.Collections.Generic;

namespace MySite.Models.ViewModels
{
    public class ProfileViewModel
    {
        public Profile Profile { get; set; }//your Profile
        public string email { get; set; }//your email
        public  PostViewModel MyPosts { get; set; }
        public bool isSubscribe { get; set; }//check sub 
        public bool Me { get; set; } = false;//check your profile or another profile
        


    }
}
