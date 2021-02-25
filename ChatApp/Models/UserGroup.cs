namespace ChatApp.Models
{
    public class UserGroup
    {
        public string GroupId { get; set; }
        public Group Group { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
