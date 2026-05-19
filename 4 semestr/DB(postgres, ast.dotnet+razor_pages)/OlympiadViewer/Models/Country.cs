using System.Collections.Generic;

namespace OlympiadViewer.Models
{
    public class Country
    {
        public string CountryCode { get; set; }

        public string Name { get; set; }

        // Navigation

        public ICollection<Participant> Participants { get; set; }
            = new List<Participant>();
    }
}