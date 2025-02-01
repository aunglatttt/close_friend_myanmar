using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class IncomeTitleModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
