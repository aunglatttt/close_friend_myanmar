using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.CashManagement
{
    public class CompanyIncomeModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }


        [DisplayName("Income Date")]
        [Required(ErrorMessage = "Income Date is required.")]
        public DateTime IncomeDate { get; set; } = DateTime.Now;


        [DisplayName("Income Title")]
        public int? IncomeTitleId  { get; set; }

        public string? IncomeTitleName { get; set; }


        [DisplayName("Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Please enter a valid number (e.g., 123 or 123.45).")]
        public int Amount { get; set; }

        [DisplayName("To Account")]
        public string? IncomeType { get; set; }
        public string? Remark { get; set; }

        public IncomeTitleModel? IncomeTitle { get; set; }

        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }

    }
}
