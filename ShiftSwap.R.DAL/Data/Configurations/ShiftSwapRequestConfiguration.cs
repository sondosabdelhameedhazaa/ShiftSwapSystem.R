using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.DAL.Models;



namespace ShiftSwap.R.DAL.Data.Configurations
{
    public class ShiftSwapRequestConfiguration : IEntityTypeConfiguration<ShiftSwapRequest>
    {
        public void Configure(EntityTypeBuilder<ShiftSwapRequest> builder)
        {
            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.SwapDate)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(500);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // Relationship: Requestor Agent (Agent who initiates swap)
            builder.HasOne(r => r.RequestorAgent)
                .WithMany(a => a.SentSwapRequests)
                .HasForeignKey(r => r.RequestorAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Target Agent (Agent to swap with)
            builder.HasOne(r => r.TargetAgent)
                .WithMany(a => a.ReceivedSwapRequests)
                .HasForeignKey(r => r.TargetAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Approved By (TeamLeader or RTM)
            builder.HasOne(r => r.ApprovedBy)
                .WithMany()
                .HasForeignKey(r => r.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
