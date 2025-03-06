using System.ComponentModel.DataAnnotations;

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
    }
}
