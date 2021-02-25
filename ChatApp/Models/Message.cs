using System;

namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public virtual User User { get; set; }
        public string UserId { get; set; }
        public virtual Group Group { get; set; }
        public string GroupId { get; set; }
        public string Image { get; set; }
    }
}
