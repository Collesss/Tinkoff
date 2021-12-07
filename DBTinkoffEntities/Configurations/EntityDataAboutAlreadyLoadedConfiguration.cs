using DBTinkoffEntities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.Configurations
{
    public class EntityDataAboutAlreadyLoadedConfiguration : IEntityTypeConfiguration<EntityDataAboutAlreadyLoaded>
    {
        void IEntityTypeConfiguration<EntityDataAboutAlreadyLoaded>.Configure(EntityTypeBuilder<EntityDataAboutAlreadyLoaded> builder)
        {
            builder.HasKey(dataAboutLoaded => new { dataAboutLoaded.Figi, dataAboutLoaded.Interval, dataAboutLoaded.Time });

            builder
                .HasOne(dataAboutLoaded => dataAboutLoaded.Stock)
                .WithMany(stock => stock.DataAboutLoadeds)
                .HasForeignKey(dataAboutLoaded => dataAboutLoaded.Figi)
                .HasPrincipalKey(stock => stock.Figi)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
