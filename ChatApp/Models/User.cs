using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ChatApp.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<Group> OwnGroups { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public DateTime LastOnline { get; set; }
        public DateTime FirstLogin { get; set; }
        public Image Image { get; set; }
        public Settings Settings { get; set; }
        public string About { get; set; }
    }
}
