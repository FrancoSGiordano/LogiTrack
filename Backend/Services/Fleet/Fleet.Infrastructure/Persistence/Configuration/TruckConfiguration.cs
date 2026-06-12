using Fleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fleet.Infrastructure.Persistence.Configuration
{
    public class TruckConfiguration : IEntityTypeConfiguration<Truck>
    {
        public void Configure(EntityTypeBuilder<Truck> builder)
        {
            builder.ToTable("Trucks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Model)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(t => t.LicensePlate)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(t => t.LicensePlate).IsUnique();

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();
        }
    }
}
