namespace Homies.Models
{
    public class EventDetailsViewModel : EventViewShortModel
    {
        public string End { get; set; } = null!;

        public string CreatedOn { get; set; } = null!;

        public string Description { get; set; } = null!;  
    }
}
