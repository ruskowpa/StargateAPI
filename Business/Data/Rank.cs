using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data
{
    [Table("Rank")]
    public class Rank
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public int Level { get; set; } // For hierarchy (1LT=1, CPT=2, MAJ=3, etc.)
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }

    public class RankConfiguration : IEntityTypeConfiguration<Rank>
    {
        public void Configure(EntityTypeBuilder<Rank> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasIndex(x => x.Abbreviation).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Abbreviation).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Level).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
