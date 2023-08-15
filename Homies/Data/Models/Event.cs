using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Homies.Data.DataConstants;

namespace Homies.Data.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(EventMaxName)]
        public string Name { get; set; }

        [Required]
        [StringLength(EventMaxDescription)]
        public string Description { get; set; }

        [Required]
        public string OrganiserId { get; set; }

        [Required]
        [ForeignKey(nameof(OrganiserId))]
        public IdentityUser Organiser { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        [ForeignKey(nameof(TypeId))]
        public Type Type { get; set; }

        //public ICollection<EventParticipant> EventsParticipants { get; set; } = new List<EventParticipant>();

    }
}
