using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models
{
    public class LogModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Logs { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LogsDate { get; set; }
        public string Type { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel CompanyInfo { get; set; }

    }
}
