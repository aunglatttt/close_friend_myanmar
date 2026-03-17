using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.ViewModel
{
    public class AccountTransferViewModel
    {
        [DisplayName("Transfer Date")]
        [Required(ErrorMessage = "Transfer date is required.")]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        [DisplayName("Amount")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public int Amount { get; set; }

        [DisplayName("From Account")]
        [Required(ErrorMessage = "From account is required.")]
        public string FromAccount { get; set; } = "Cash";

        [DisplayName("To Account")]
        [Required(ErrorMessage = "To account is required.")]
        public string ToAccount { get; set; } = "APM AYA Bank";

        [DisplayName("Remark")]
        public string? Description { get; set; }
    }
}
