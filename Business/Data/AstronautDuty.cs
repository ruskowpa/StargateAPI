using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("AstronautDuty")]
    public class AstronautDuty
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public int RankId { get; set; }

        public int DutyTitleId { get; set; }

        public DateTime DutyStartDate { get; set; }

        public DateTime? DutyEndDate { get; set; }

        // Navigation properties
        public virtual Person Person { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual DutyTitle DutyTitle { get; set; }
    }

    public class AstronautDutyConfiguration : IEntityTypeConfiguration<AstronautDuty>
    {
        public void Configure(EntityTypeBuilder<AstronautDuty> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Foreign key relationships
            builder.HasOne(x => x.Person)
                .WithMany(x => x.AstronautDuties)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(x => x.Rank)
                .WithMany(x => x.AstronautDuties)
                .HasForeignKey(x => x.RankId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.DutyTitle)
                .WithMany(x => x.AstronautDuties)
                .HasForeignKey(x => x.DutyTitleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
