using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations.Schema;

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


        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }

    }
}
