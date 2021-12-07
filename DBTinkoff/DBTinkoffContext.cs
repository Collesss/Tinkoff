using DBTinkoffEntities.Configurations;
using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DBTinkoff
{
    public class DBTinkoffContext : DbContext
    {
        public DbSet<EntityMarketInstrument> Stoks { get; set; }
        public DbSet<EntityCandlePayload> Candles { get; set; }
        public DbSet<EntityDataAboutAlreadyLoaded> DataAboutLoadeds { get; set; }

        /*
        public DBTinkoffContext()
        {
            Database.EnsureCreated();
        }
        */

        public DBTinkoffContext(DbContextOptions<DBTinkoffContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data source=test.db");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EntityMarketInstrumentConfiguration());
            modelBuilder.ApplyConfiguration(new EntityCandlePayloadConfiguration());
            modelBuilder.ApplyConfiguration(new EntityDataAboutAlreadyLoadedConfiguration());
        }
    }
}
