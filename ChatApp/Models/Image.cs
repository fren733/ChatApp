namespace ChatApp.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string ImageName { get; set; }
        public byte[] ByteString { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
    }
}
