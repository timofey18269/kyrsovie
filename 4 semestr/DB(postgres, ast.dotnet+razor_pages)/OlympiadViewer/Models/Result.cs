namespace OlympiadViewer.Models
{
    public class Result
    {
        public int ResultId { get; set; }

        public int SportId { get; set; }

        public int ParticipantId { get; set; }

        public int StartId { get; set; }

        public int Place { get; set; }

        public string ResultValue { get; set; }

        // Navigation

        public Sport Sport { get; set; }

        public Participant Participant { get; set; }

        public EventSchedule EventSchedule { get; set; }
    }
}