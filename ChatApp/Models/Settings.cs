namespace ChatApp.Models
{
    public class Settings
    {
        public Settings() { }
        public Settings(bool darkMode)
        {
            DarkMode = darkMode;
        }

        public int Id { get; set; }
        public bool DarkMode { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
