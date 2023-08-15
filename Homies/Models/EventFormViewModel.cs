using System.ComponentModel.DataAnnotations;
using static Homies.Data.DataConstants;

namespace Homies.Models
{
    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(EventMaxName, MinimumLength = EventMinName,
            ErrorMessage = "Event name must be between 5 and 20 characters.")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(EventMaxDescription, MinimumLength = EventMinDescription,
            ErrorMessage = "Description must be between 15 and 150 characters.")]
        public string Description { get; set; } = null!;
        
        [Required]
        public DateTime Start { get; set; }
        
        [Required]
        public DateTime End { get; set; }
        
        [Required]
        public int TypeId { get; set; }

        public IEnumerable<EventTypeViewModel> Types { get; set; }
            = new List<EventTypeViewModel>();
    }
}
