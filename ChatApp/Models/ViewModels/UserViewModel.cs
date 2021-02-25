using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.ViewModels
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public bool IsFriend { get; set; }
        public bool IsOnline { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastOnline { get; set; }
        public IFormFile Image { get; set; }
        public string Source { get; set; }
    }
}
