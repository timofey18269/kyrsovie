using System;
using System.Collections.Generic;

namespace OlympiadViewer.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }

        public string CountryCode { get; set; }

        public int SportId { get; set; }

        public string FullName { get; set; }

        public DateOnly BirthDate { get; set; }

        // Navigation

        public Country Country { get; set; }

        public Sport Sport { get; set; }

        public ICollection<Result> Results { get; set; }
            = new List<Result>();
    }
}