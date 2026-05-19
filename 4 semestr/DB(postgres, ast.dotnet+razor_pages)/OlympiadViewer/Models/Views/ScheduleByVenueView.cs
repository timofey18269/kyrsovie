using System;

namespace OlympiadViewer.Models.Views
{
    public class ScheduleByVenueView
    {
        public DateOnly StartDate { get; set; }

        public string VenueName { get; set; }

        public string SportName { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}