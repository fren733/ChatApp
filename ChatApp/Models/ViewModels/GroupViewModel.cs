using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.ViewModels
{
    public class GroupViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string About { get; set; }
        public string LastOnline { get; set; }
        public string Src { get; set; }
        public bool AmIKing { get; set; }
        public List<UserViewModel> Users { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
