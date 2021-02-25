namespace ChatApp.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public bool DarkMode { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
