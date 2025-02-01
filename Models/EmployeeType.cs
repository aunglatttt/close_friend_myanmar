using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class EmployeeType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
