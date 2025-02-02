using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models
{
    public class PropertyTypeModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        public string ShortCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
