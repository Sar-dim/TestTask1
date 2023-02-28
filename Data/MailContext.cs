using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class MailContext : DbContext
    {
        public DbSet<Letter> Letters { get; set; }

        public MailContext(DbContextOptions<MailContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public MailContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("MailSQLConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
