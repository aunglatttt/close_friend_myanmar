using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class IncomeTitleModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Remark { get; set; }
    }
}
