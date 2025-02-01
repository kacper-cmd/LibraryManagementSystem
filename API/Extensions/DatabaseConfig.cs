using Database;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class DatabaseConfig
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextFactory<ApplicationDbContext>(
             options => options.UseSqlServer(cs));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                 options.UseSqlServer(cs ?? throw new InvalidOperationException("Connection string not found"));
			     options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			});         
        }
    }
}
