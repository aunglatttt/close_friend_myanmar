using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class BuildingTypeModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
