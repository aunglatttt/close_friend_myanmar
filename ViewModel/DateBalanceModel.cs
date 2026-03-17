using CloseFriendMyanamr.Models.CashManagement;

namespace CloseFriendMyanamr.ViewModel
{
    public class DateBalanceModel
    {
        public DateTime Date { get; set; }
        public decimal CashOpeningBalance { get; set; }
        public decimal CashClosingBalance { get; set; }
        public decimal BankOpeningBalance { get; set; }
        public decimal BankClosingBalance { get; set; }
    }
}
