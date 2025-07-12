using ClinicalTrial2._0.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial2._0.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trial> Trials { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TrialTreatment> TrialTreatments { get; set; }
        public DbSet<TrialTranslation> TrialTranslations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trial>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTrials)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Participant)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes for better performance
            builder.Entity<Trial>()
                .HasIndex(t => t.NCTNumber)
                .IsUnique();

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.ParticipantId, e.TrialId })
                .IsUnique();

            // Seed data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Locations
            builder.Entity<Location>().HasData(
                new Location { LocationId = 1, City = "Cape Town", Province = "Western Cape", Country = "South Africa", PostalCode = "8001" },
                new Location { LocationId = 2, City = "Johannesburg", Province = "Gauteng", Country = "South Africa", PostalCode = "2000" },
                new Location { LocationId = 3, City = "Durban", Province = "KwaZulu-Natal", Country = "South Africa", PostalCode = "4000" },
                new Location { LocationId = 4, City = "Pretoria", Province = "Gauteng", Country = "South Africa", PostalCode = "0001" },
                new Location { LocationId = 5, City = "Port Elizabeth", Province = "Eastern Cape", Country = "South Africa", PostalCode = "6000" }
            );

            // Seed Diseases
            builder.Entity<Disease>().HasData(
                new Disease { DiseaseId = 1, DiseaseName = "Hypertension", Description = "High blood pressure", Category = "Cardiovascular" },
                new Disease { DiseaseId = 2, DiseaseName = "Diabetes Type 2", Description = "Type 2 diabetes mellitus", Category = "Endocrine" },
                new Disease { DiseaseId = 3, DiseaseName = "HIV/AIDS", Description = "Human Immunodeficiency Virus", Category = "Infectious Disease" },
                new Disease { DiseaseId = 4, DiseaseName = "Tuberculosis", Description = "Tuberculosis infection", Category = "Infectious Disease" },
                new Disease { DiseaseId = 5, DiseaseName = "Cancer", Description = "Various forms of cancer", Category = "Oncology" },
                new Disease { DiseaseId = 6, DiseaseName = "Depression", Description = "Major depressive disorder", Category = "Mental Health" },
                new Disease { DiseaseId = 7, DiseaseName = "Asthma", Description = "Chronic respiratory condition", Category = "Respiratory" }
            );

            // Seed Treatments
            builder.Entity<Treatment>().HasData(
                new Treatment { TreatmentId = 1, TreatmentName = "ACE Inhibitors", Description = "Blood pressure medication", TreatmentType = "Medication" },
                new Treatment { TreatmentId = 2, TreatmentName = "Metformin", Description = "Diabetes medication", TreatmentType = "Medication" },
                new Treatment { TreatmentId = 3, TreatmentName = "Antiretroviral Therapy", Description = "HIV treatment", TreatmentType = "Medication" },
                new Treatment { TreatmentId = 4, TreatmentName = "Chemotherapy", Description = "Cancer treatment", TreatmentType = "Therapy" },
                new Treatment { TreatmentId = 5, TreatmentName = "Cognitive Behavioral Therapy", Description = "Depression treatment", TreatmentType = "Therapy" },
                new Treatment { TreatmentId = 6, TreatmentName = "Inhaled Corticosteroids", Description = "Asthma treatment", TreatmentType = "Medication" },
                new Treatment { TreatmentId = 7, TreatmentName = "Behavioral Intervention", Description = "Lifestyle modification", TreatmentType = "Intervention" }
            );
        }
    }
}
