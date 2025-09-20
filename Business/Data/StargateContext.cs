using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StargateAPI.Business.Data
{
    public class StargateContext : DbContext
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<Person> People { get; set; }
        public DbSet<AstronautDetail> AstronautDetails { get; set; }
        public DbSet<AstronautDuty> AstronautDuties { get; set; }
        public DbSet<DutyTitle> DutyTitles { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        public StargateContext(DbContextOptions<StargateContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Duty Titles
            modelBuilder.Entity<DutyTitle>()
                .HasData(
                    new DutyTitle { Id = 1, Title = "Commander", Description = "Unit commanding officer", IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new DutyTitle { Id = 2, Title = "Captain", Description = "Company-level command", IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new DutyTitle { Id = 3, Title = "Major", Description = "Battalion-level command", IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new DutyTitle { Id = 4, Title = "Lieutenant", Description = "Platoon-level command", IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new DutyTitle { Id = 5, Title = "RETIRED", Description = "Retired from active service", IsActive = true, CreatedDate = new DateTime(2020, 1, 1) }
                );

            // Seed Ranks
            modelBuilder.Entity<Rank>()
                .HasData(
                    new Rank { Id = 1, Name = "Second Lieutenant", Abbreviation = "2LT", Level = 1, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new Rank { Id = 2, Name = "First Lieutenant", Abbreviation = "1LT", Level = 2, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new Rank { Id = 3, Name = "Captain", Abbreviation = "CPT", Level = 3, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new Rank { Id = 4, Name = "Major", Abbreviation = "MAJ", Level = 4, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new Rank { Id = 5, Name = "Lieutenant Colonel", Abbreviation = "LTC", Level = 5, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) },
                    new Rank { Id = 6, Name = "Colonel", Abbreviation = "COL", Level = 6, IsActive = true, CreatedDate = new DateTime(2020, 1, 1) }
                );

            // Seed People
            modelBuilder.Entity<Person>()
                .HasData(
                    new Person { Id = 1, Name = "John Doe" },
                    new Person { Id = 2, Name = "Jane Doe" }
                );

            // Seed Astronaut Details
            modelBuilder.Entity<AstronautDetail>()
                .HasData(
                    new AstronautDetail
                    {
                        Id = 1,
                        PersonId = 1,
                        CareerStartDate = new DateTime(2020, 1, 1)
                    }
                );

            // Seed Astronaut Duties (using foreign keys)
            modelBuilder.Entity<AstronautDuty>()
                .HasData(
                    new AstronautDuty
                    {
                        Id = 1,
                        PersonId = 1,
                        RankId = 2, // 1LT
                        DutyTitleId = 1, // Commander
                        DutyStartDate = new DateTime(2020, 1, 1)
                    }
                );
        }
    }
}
