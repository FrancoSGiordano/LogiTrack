using Fleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fleet.Infrastructure.Persistence.Configuration
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Origin).HasMaxLength(255).IsRequired();
            builder.Property(t => t.Destination).HasMaxLength(255).IsRequired();

            builder.Property(t => t.OriginLat).IsRequired();
            builder.Property(t => t.OriginLon).IsRequired();

            builder.Property(t => t.DestinationLat).IsRequired();
            builder.Property(t => t.DestinationLon).IsRequired();

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired(); 
        }
    }
}
