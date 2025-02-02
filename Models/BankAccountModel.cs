using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class BankAccountModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BankAccount { get; set; }
        [Required]
        public int OpeningAmount { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
