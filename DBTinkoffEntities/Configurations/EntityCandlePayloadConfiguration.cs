using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBTinkoffEntities.Configurations
{
    public class EntityCandlePayloadConfiguration : IEntityTypeConfiguration<EntityCandlePayload>
    {
        public void Configure(EntityTypeBuilder<EntityCandlePayload> builder)
        {
            builder.HasKey(candle => new { candle.Figi, candle.Time, candle.Interval });

            builder.Property(candle => candle.Open);
            builder.Property(candle => candle.Close);
            builder.Property(candle => candle.High);
            builder.Property(candle => candle.Low);
            builder.Property(candle => candle.Volume);

            builder
                .HasOne(candle => candle.Stock)
                .WithMany(stock => stock.Candles)
                .HasForeignKey(candle => candle.Figi)
                .HasPrincipalKey(stock => stock.Figi)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
