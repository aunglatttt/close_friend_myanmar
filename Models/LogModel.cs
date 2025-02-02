using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class LogModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Logs { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LogsDate { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual EmployeeModel Employee { get; set; }
    }
}
