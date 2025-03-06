
using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class FacilityModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
