namespace CloseFriendMyanamr.ViewModel
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        public string AvaliableDate { get; set; }
        public string? Code { get; set; }
        public string Status { get; set; }
        public string Township { get; set; }
        public string? Street { get; set; }
        public string? Comment { get; set; }
        public string? Room { get; set; }
        public decimal Price { get; set; }
        public string Owner { get; set; }
        public string? Remark { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public string LastCheckedBy { get; set; }
    }
}
