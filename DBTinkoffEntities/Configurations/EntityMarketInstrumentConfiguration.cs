using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBTinkoffEntities.Configurations
{
    public class EntityMarketInstrumentConfiguration : IEntityTypeConfiguration<EntityMarketInstrument>
    {
        public void Configure(EntityTypeBuilder<EntityMarketInstrument> builder)
        {
            builder.HasKey(stock => stock.Figi);

            builder.HasAlternateKey(stock => stock.Ticker);
            builder.HasAlternateKey(stock => stock.Isin);

            builder.Property(stock => stock.MinPriceIncrement);
            builder.Property(stock => stock.Lot);
            builder.Property(stock => stock.Currency);
            builder.Property(stock => stock.Type);


            builder.HasIndex(stock => stock.Name);

            builder.Property(stock => stock.Name)
                .IsRequired();
        }
    }
}
