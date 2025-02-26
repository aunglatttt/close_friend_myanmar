using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CloseFriendMyanamr.Models.CashManagement
{
    public class CashBookTransaction : BaseDomain
    {
        [Key]
        public int Id { get; set; }


        [DisplayName("Transaction Date")]
        [Required(ErrorMessage = "Transaction Date is required.")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;


        [DisplayName("Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Please enter a valid number (e.g., 123 or 123.45).")]
        public int Amount { get; set; }


        [DisplayName("Transaction Type")]
        [Required(ErrorMessage = "Transaction Type is required.")]
        public string TransactionType { get; set; }

        public string? Description { get; set; }

        public string? Account { get; set; }

    }
}
