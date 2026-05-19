using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Models;
using OlympiadViewer.Models.Views;
//using OlympiadViewer.Models.Views;

namespace OlympiadViewer.Data
{
    public class OlympiadContext : DbContext
    {
        public OlympiadContext(DbContextOptions<OlympiadContext> options)
            : base(options)
        {
        }

        // =========================
        // TABLES
        // =========================

        public DbSet<Country> Countries { get; set; }

        public DbSet<Sport> Sports { get; set; }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<EventSchedule> EventSchedules { get; set; }

        public DbSet<Result> Results { get; set; }


        // =========================
        // VIEWS
        // =========================

        public DbSet<CountryMedalsView> CountryMedalsView { get; set; }

        public DbSet<AthleteResultsView> AthleteResultsView { get; set; }

        public DbSet<AverageAgeView> AverageAgeView { get; set; }

        public DbSet<ScheduleByVenueView> ScheduleByVenueView { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // COUNTRIES
            // =========================================================

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("countries");

                entity.HasKey(e => e.CountryCode);

                entity.Property(e => e.CountryCode)
                    .HasColumnName("country_code")
                    .HasMaxLength(3);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
            });


            // =========================================================
            // SPORTS
            // =========================================================

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.ToTable("sports");

                entity.HasKey(e => e.SportId);

                entity.Property(e => e.SportId)
                    .HasColumnName("sport_id");

                entity.Property(e => e.SportName)
                    .HasColumnName("sport_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.SportType)
                    .HasColumnName("sport_type")
                    .HasMaxLength(50)
                    .IsRequired();
            });


            // =========================================================
            // VENUES
            // =========================================================

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.ToTable("venues");

                entity.HasKey(e => e.VenueId);

                entity.Property(e => e.VenueId)
                    .HasColumnName("venue_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(200);

                entity.Property(e => e.PossibleSports)
                    .HasColumnName("possible_sports");
            });


            // =========================================================
            // PARTICIPANTS
            // =========================================================

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("participants");

                entity.HasKey(e => e.ParticipantId);

                entity.Property(e => e.ParticipantId)
                    .HasColumnName("participant_id");

                entity.Property(e => e.CountryCode)
                    .HasColumnName("country_code")
                    .HasMaxLength(3);

                entity.Property(e => e.SportId)
                    .HasColumnName("sport_id");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.BirthDate)
                    .HasColumnName("birth_date");

                // RELATIONS

                entity.HasOne(e => e.Country)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(e => e.CountryCode);

                entity.HasOne(e => e.Sport)
                    .WithMany(s => s.Participants)
                    .HasForeignKey(e => e.SportId);
            });


            // =========================================================
            // EVENT SCHEDULE
            // =========================================================

            modelBuilder.Entity<EventSchedule>(entity =>
            {
                entity.ToTable("event_schedule");

                entity.HasKey(e => e.StartId);

                entity.Property(e => e.StartId)
                    .HasColumnName("start_id");

                entity.Property(e => e.SportId)
                    .HasColumnName("sport_id");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time");

                entity.Property(e => e.VenueId)
                    .HasColumnName("venue_id");

                // RELATIONS

                entity.HasOne(e => e.Sport)
                    .WithMany(s => s.EventSchedules)
                    .HasForeignKey(e => e.SportId);

                entity.HasOne(e => e.Venue)
                    .WithMany(v => v.EventSchedules)
                    .HasForeignKey(e => e.VenueId);
            });


            // =========================================================
            // RESULTS
            // =========================================================

            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("results");

                entity.HasKey(e => e.ResultId);

                entity.Property(e => e.ResultId)
                    .HasColumnName("result_id");

                entity.Property(e => e.SportId)
                    .HasColumnName("sport_id");

                entity.Property(e => e.ParticipantId)
                    .HasColumnName("participant_id");

                entity.Property(e => e.StartId)
                    .HasColumnName("start_id");

                entity.Property(e => e.Place)
                    .HasColumnName("place");

                entity.Property(e => e.ResultValue)
                    .HasColumnName("result")
                    .HasMaxLength(100);

                // UNIQUE CONSTRAINT

                entity.HasIndex(e => new
                {
                    e.ParticipantId,
                    e.StartId
                }).IsUnique();

                // RELATIONS

                entity.HasOne(e => e.Sport)
                    .WithMany(s => s.Results)
                    .HasForeignKey(e => e.SportId);

                entity.HasOne(e => e.Participant)
                    .WithMany(p => p.Results)
                    .HasForeignKey(e => e.ParticipantId);

                entity.HasOne(e => e.EventSchedule)
                    .WithMany(es => es.Results)
                    .HasForeignKey(e => e.StartId);
            });


            // =========================================================
            // VIEW: vw_country_medals
            // =========================================================

            modelBuilder.Entity<CountryMedalsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_country_medals");

                entity.Property(e => e.CountryCode)
                    .HasColumnName("country_code");

                entity.Property(e => e.CountryName)
                    .HasColumnName("country_name");

                entity.Property(e => e.TotalMedals)
                    .HasColumnName("total_medals");

                entity.Property(e => e.GoldMedals)
                    .HasColumnName("gold_medals");

                entity.Property(e => e.SilverMedals)
                    .HasColumnName("silver_medals");

                entity.Property(e => e.BronzeMedals)
                    .HasColumnName("bronze_medals");
            });


            // =========================================================
            // VIEW: vw_athlete_results
            // =========================================================

            modelBuilder.Entity<AthleteResultsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_athlete_results");

                entity.Property(e => e.ParticipantId)
                    .HasColumnName("participant_id");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name");

                entity.Property(e => e.Country)
                    .HasColumnName("country");

                entity.Property(e => e.SportName)
                    .HasColumnName("sport_name");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time");

                entity.Property(e => e.Venue)
                    .HasColumnName("venue");

                entity.Property(e => e.Place)
                    .HasColumnName("place");

                entity.Property(e => e.Result)
                    .HasColumnName("result");
            });


            // =========================================================
            // VIEW: vw_average_age_by_sport
            // =========================================================

            modelBuilder.Entity<AverageAgeView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_average_age_by_sport");

                entity.Property(e => e.SportId)
                    .HasColumnName("sport_id");

                entity.Property(e => e.SportName)
                    .HasColumnName("sport_name");

                entity.Property(e => e.AverageAge)
                    .HasColumnName("average_age");
            });


            // =========================================================
            // VIEW: vw_schedule_by_venue
            // =========================================================

            modelBuilder.Entity<ScheduleByVenueView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_schedule_by_venue");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date");

                entity.Property(e => e.VenueName)
                    .HasColumnName("venue_name");

                entity.Property(e => e.SportName)
                    .HasColumnName("sport_name");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time");
            });
        }
    }
}