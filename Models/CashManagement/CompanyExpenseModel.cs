using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.CashManagement
{
    public class CompanyExpenseModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }


        [DisplayName("Expense Date")]
        [Required(ErrorMessage = "Expense Date is required.")]
        public DateTime ExpenseDate { get; set; } = DateTime.Now;


        [DisplayName("Expense Title")]
        public int? ExpenseTitleId { get; set; }

        public string? ExpenseTitleName { get; set; }


        [DisplayName("Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Please enter a valid number (e.g., 123 or 123.45).")]
        public int Amount { get; set; }

        [DisplayName("From Account")]
        public string? ExpenseType { get; set; }
        public string? Description { get; set; }

        public ExpenseTitleModel? ExpenseTitle { get; set; }


        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }
    }
}
