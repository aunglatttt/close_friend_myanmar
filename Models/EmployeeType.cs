using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models
{
    public class EmployeeType : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel CompanyInfo { get; set; }
    }
}
