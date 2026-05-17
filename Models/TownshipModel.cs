using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class TownshipModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Township { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TownshipMM { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
