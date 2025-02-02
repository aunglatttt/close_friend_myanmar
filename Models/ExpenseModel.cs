using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class ExpenseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
