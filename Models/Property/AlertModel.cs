namespace CloseFriendMyanamr.Models.Property
{
    public class AlertModel
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
    }
}
