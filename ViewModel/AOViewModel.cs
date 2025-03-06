namespace CloseFriendMyanamr.ViewModel
{
    public class AOViewModel
    {
        public int Id { get; set; }
        public string OwnerName { get; set; }
        public string? OwnerPhone { get; set; }
        public string? Address { get; set; }
        public string? Remark { get; set; }
        public int PropertyCount { get; set; } = 0;
    }
}
