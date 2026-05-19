using System.Collections.Generic;

namespace OlympiadViewer.Models
{
    public class Sport
    {
        public int SportId { get; set; }

        public string SportName { get; set; }

        public string SportType { get; set; }

        // Navigation

        public ICollection<Participant> Participants { get; set; }
            = new List<Participant>();

        public ICollection<EventSchedule> EventSchedules { get; set; }
            = new List<EventSchedule>();

        public ICollection<Result> Results { get; set; }
            = new List<Result>();
    }
}