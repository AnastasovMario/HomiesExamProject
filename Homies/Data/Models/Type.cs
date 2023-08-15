using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants;

namespace Homies.Data.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(TypeNameMax)]
        public string Name { get; set; } = null!;

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
