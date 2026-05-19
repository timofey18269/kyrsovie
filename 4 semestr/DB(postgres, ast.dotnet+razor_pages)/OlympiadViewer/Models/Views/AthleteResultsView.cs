using System;

namespace OlympiadViewer.Models.Views
{
    public class AthleteResultsView
    {
        public int ParticipantId { get; set; }

        public string FullName { get; set; }

        public string Country { get; set; }

        public string SportName { get; set; }

        public DateOnly StartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string Venue { get; set; }

        public int Place { get; set; }

        public string Result { get; set; }
    }
}