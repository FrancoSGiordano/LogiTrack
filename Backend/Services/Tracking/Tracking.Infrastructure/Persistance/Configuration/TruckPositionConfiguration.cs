using Microsoft.EntityFrameworkCore;
using Tracking.Core.Entities;

namespace Tracking.Infrastructure.Persistance.Configuration
{
    public class TruckPositionConfiguration : IEntityTypeConfiguration<TruckPosition>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TruckPosition> builder)
        {
            builder.ToTable("TruckPositions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Latitude).IsRequired();
            builder.Property(t => t.Longitude).IsRequired();
            builder.Property(t => t.Timestamp).IsRequired();

            builder.HasIndex(t => new { t.TruckId, t.Timestamp });

            builder.HasIndex(t => t.TripId);
        }
    }
}
