using System;
using System.Collections.Generic;

namespace OlympiadViewer.Models
{
    public class EventSchedule
    {
        public int StartId { get; set; }

        public int SportId { get; set; }

        public DateOnly StartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int VenueId { get; set; }

        // Navigation

        public Sport Sport { get; set; }

        public Venue Venue { get; set; }

        public ICollection<Result> Results { get; set; }
            = new List<Result>();
    }
}