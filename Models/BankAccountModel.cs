using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models
{
    public class BankAccountModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BankAccount { get; set; }
        [Required]
        public int OpeningAmount { get; set; }
        public string? Remark { get; set; }
        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel CompanyInfo { get; set; }

    }
}
