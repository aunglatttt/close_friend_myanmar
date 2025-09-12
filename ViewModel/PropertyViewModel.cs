namespace CloseFriendMyanamr.ViewModel
{
    public class PropertyViewModel
    {
        //public int Id { get; set; }
        //public string AvaliableDate { get; set; }
        //public string? Code { get; set; }
        //public string Status { get; set; }
        //public string Township { get; set; }
        //public string? Street { get; set; }
        //public string? Comment { get; set; }
        //public string? Room { get; set; }
        //public decimal SalePrice { get; set; }
        //public decimal RentPrice { get; set; }
        //public string Owner { get; set; }
        //public string? Remark { get; set; }
        //public DateTime LastCheckedDate { get; set; }
        //public string LastCheckedBy { get; set; }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Township { get; set; } // added
        public string? Room { get; set; } // added
        public decimal RentPrice { get; set; } // added
        public string AvaliableDate { get; set; }
        public DateTime LastCheckedDate { get; set; } // Keep this if you need the DateTime object for other logic
        public string LastCheckedDateString { get; set; } // Add this for display in the table
        public string Street { get; set; }
        public string Comment { get; set; }
        public decimal? SalePrice { get; set; }
        public string Owner { get; set; }
        public string Remark { get; set; }
        public string LastCheckedBy { get; set; }
    }
}
