using System;
using System.Collections.Generic;

namespace ChatApp.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string About { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastOnline { get; set; }
        public User Owner { get; set; }
        public Image Image { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
