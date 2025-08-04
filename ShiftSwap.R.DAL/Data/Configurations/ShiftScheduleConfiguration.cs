using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.DAL.Data.Configurations
{
    public class ShiftScheduleConfiguration : IEntityTypeConfiguration<ShiftSchedule>
    {
        public void Configure(EntityTypeBuilder<ShiftSchedule> builder)
        {
            // Primary Key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Date)
                .IsRequired();

            builder.Property(s => s.ShiftStart)
                .IsRequired();

            builder.Property(s => s.ShiftEnd)
                .IsRequired();

            builder.Property(s => s.Shift)
                .HasMaxLength(50);

            builder.Property(s => s.LOB)
                .HasMaxLength(100);

            builder.Property(s => s.Schedule)
                .HasMaxLength(200);

            // Relationship: Schedule -> Agent
            builder.HasOne(s => s.Agent)
                .WithMany(a => a.ShiftSchedules)
                .HasForeignKey(s => s.AgentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
