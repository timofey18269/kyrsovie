using System.Collections.Generic;

namespace OlympiadViewer.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        // PostgreSQL integer[]
        public int[] PossibleSports { get; set; }

        // Navigation

        public ICollection<EventSchedule> EventSchedules { get; set; }
            = new List<EventSchedule>();
    }
}