using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models.CompanyInformation
{
    public class CompanyInfoModel
    {
        [Key]
        public int CPI { get; set; }
        public string CompanyName { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string ShortName { get; set; }
        public bool Status { get; set; }
        public bool IsAddressShow { get; set; }
        public bool IsTownshipShow { get; set; }
        public bool IsInfoShow { get; set; }
        public bool IsRoomShow { get; set; }
        public bool IsSalePriceShow { get; set; }
        public bool IsRentPriceShow { get; set; }
        public bool IsRemarkShow { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
