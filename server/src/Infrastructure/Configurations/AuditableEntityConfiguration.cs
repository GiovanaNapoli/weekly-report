using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property<string>("CreatedBy")
            .HasColumnName("TX_CREATED_BY")
            .IsRequired();

        builder.Property<DateTime>("CreatedAt")
            .HasColumnName("DT_CREATED_AT")
            .IsRequired();

        builder.Property<string>("UpdatedBy")
            .HasColumnName("TX_UPDATED_BY");

        builder.Property<DateTime?>("UpdatedAt")
            .HasColumnName("DT_UPDATED_AT");
        
        builder.Property<bool> ("IsActive")
            .HasColumnName("FL_ACTIVE")
            .HasDefaultValue(true);
    }
}