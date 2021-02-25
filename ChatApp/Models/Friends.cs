namespace ChatApp.Models
{
    public class Friends
    {
        public int Id { get; set; }
        public string FirstUserId { get; set; }
        public string SecondUserId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
