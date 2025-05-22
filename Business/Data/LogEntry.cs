using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data
{
	[Table("LogEntry")]
	public class LogEntry
	{
        public int Id { get; set; }

		public string LogMessage { get; set; } = string.Empty;

        public DateTime LogTimeStamp { get; set; }

		public LogLevel LogLevel { get; set; } 
    }

    public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }

    public enum LogLevel
	{
		INFO,
		DEBUG,
		ERROR

	}
}

