using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CloseFriendMyanamr.Models.CashManagement
{
    public class CompanyIncome : BaseDomain
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
        public double Amount { get; set; }

        [DisplayName("To Account")]
        public string? IncomeType { get; set; }
        public string? Remark { get; set; }

        public IncomeTitleModel? IncomeTitle { get; set; }
    }
}
