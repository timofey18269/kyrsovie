namespace OlympiadViewer.Models.Views
{
    public class CountryMedalsView
    {
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public int TotalMedals { get; set; }

        public int GoldMedals { get; set; }

        public int SilverMedals { get; set; }

        public int BronzeMedals { get; set; }
    }
}