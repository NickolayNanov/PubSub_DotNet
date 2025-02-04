using Microsoft.EntityFrameworkCore;

namespace Test.PubSub.Net.WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

}
