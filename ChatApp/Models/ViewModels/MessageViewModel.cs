using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
        public string ProfileImage { get; set; }
    }
}
