using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class BankAccountInfoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BankName { get; set; }
        [Required]
        public double OpeningAmount { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
