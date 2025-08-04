using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.BLL.Repositories;
using ShiftSwap.R.BLL;

namespace ShiftSwap.R.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register DbContext
            builder.Services.AddDbContext<ShiftSwapDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories & UnitOfWork
            builder.Services.AddScoped<IAgentRepository, AgentRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IShiftScheduleRepository, ShiftScheduleRepository>();
            builder.Services.AddScoped<IShiftSwapRequestRepository, ShiftSwapRequestRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();











            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
