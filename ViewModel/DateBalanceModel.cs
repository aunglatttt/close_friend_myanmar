using CloseFriendMyanamr.Models.CashManagement;

namespace CloseFriendMyanamr.ViewModel
{
    public class DateBalanceModel
    {
        public DateTime Date { get; set; }
        public double CashOpeningBalance { get; set; }
        public double CashClosingBalance { get; set; }
        public double BankOpeningBalance { get; set; }
        public double BankClosingBalance { get; set; }
    }
}
