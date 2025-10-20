using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.Appointment
{
    public class VisitorTracking
    {
        public int Id { get; set; }
        public string? PageUrl { get; set; }
        public string? Referrer { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? DeviceType { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Browser { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public string Timezone { get; set; }
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel CompanyInfo { get; set; }
    }
}
