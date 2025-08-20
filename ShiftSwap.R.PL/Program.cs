using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.BLL.Repositories;
using ShiftSwap.R.BLL;
using ShiftSwap.R.DAL.Data;

namespace ShiftSwap.R.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // Register DbContext
            builder.Services.AddDbContext<ShiftSwapDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories & UnitOfWork
            builder.Services.AddScoped<IAgentRepository, AgentRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IShiftScheduleRepository, ShiftScheduleRepository>();
            builder.Services.AddScoped<IShiftSwapRequestRepository, ShiftSwapRequestRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // Enable Windows Authentication (for NTName)
            builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust session timeout as needed
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Seed Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ShiftSwapDbContext>();
                context.Database.Migrate();
                DbInitializer.Seed(context);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();



            app.UseRouting();

            app.UseSession();            
            app.UseAuthentication();     
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


           
            app.Run();
        }
    }
}

