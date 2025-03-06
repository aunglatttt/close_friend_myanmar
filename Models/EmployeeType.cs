using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class EmployeeType : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        public bool ReadOnly { get; set; }
    }
}
