using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data
{
    [Table("LogEntry")]
    public class LogEntry
    {
        public int Id { get; set; }
        public string LogLevel { get; set; } = string.Empty; // INFO, WARN, ERROR, DEBUG, TRACE
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; } // Stack trace and exception details
        public string? Source { get; set; } // Controller, Service, etc.
        public string? Method { get; set; } // Method name where log occurred
        public string? UserId { get; set; } // Optional user context
        public string? RequestId { get; set; } // Optional request correlation
        public string? AdditionalData { get; set; } // JSON for additional context
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? MachineName { get; set; }
        public string? Environment { get; set; }
    }

    public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.LogLevel)
                .HasMaxLength(20)
                .IsRequired();
                
            builder.Property(x => x.Message)
                .HasMaxLength(4000)
                .IsRequired();
                
            builder.Property(x => x.Exception)
                .HasMaxLength(8000); // Large enough for stack traces
                
            builder.Property(x => x.Source)
                .HasMaxLength(200);
                
            builder.Property(x => x.Method)
                .HasMaxLength(200);
                
            builder.Property(x => x.UserId)
                .HasMaxLength(100);
                
            builder.Property(x => x.RequestId)
                .HasMaxLength(100);
                
            builder.Property(x => x.AdditionalData)
                .HasMaxLength(2000); // JSON data
                
            builder.Property(x => x.Timestamp)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            builder.Property(x => x.MachineName)
                .HasMaxLength(100);
                
            builder.Property(x => x.Environment)
                .HasMaxLength(50);
                
            // Indexes for common queries
            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => x.LogLevel);
            builder.HasIndex(x => x.Source);
            builder.HasIndex(x => new { x.LogLevel, x.Timestamp });
        }
    }
}
