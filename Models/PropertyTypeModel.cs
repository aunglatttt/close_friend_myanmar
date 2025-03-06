using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class PropertyTypeModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        public string ShortCode { get; set; }
    }
}
