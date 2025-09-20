using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data
{
    [Table("DutyTitle")]
    public class DutyTitle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }

    public class DutyTitleConfiguration : IEntityTypeConfiguration<DutyTitle>
    {
        public void Configure(EntityTypeBuilder<DutyTitle> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Title).IsUnique();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
