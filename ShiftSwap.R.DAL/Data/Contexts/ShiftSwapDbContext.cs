using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.DAL.Data.Configurations;
using Microsoft.EntityFrameworkCore;



namespace ShiftSwap.R.DAL.Data.Contexts
{
    public class ShiftSwapDbContext : DbContext
    {
        public ShiftSwapDbContext(DbContextOptions<ShiftSwapDbContext> options)
            : base(options)
        {
        }

        // DbSets (Tables)
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ShiftSchedule> ShiftSchedules { get; set; }
        public DbSet<ShiftSwapRequest> ShiftSwapRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Configurations
            modelBuilder.ApplyConfiguration(new AgentConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ShiftScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new ShiftSwapRequestConfiguration());
        }
    }
}
