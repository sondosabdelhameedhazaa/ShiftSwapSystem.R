using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.DAL.Data.Configurations
{
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.HRID)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.LoginID)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.NTName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Role)
                .IsRequired();

            builder.HasOne(a => a.Project)
                .WithMany(p => p.Agents)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.TeamLeader)
                .WithMany()
                .HasForeignKey(a => a.TeamLeaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

