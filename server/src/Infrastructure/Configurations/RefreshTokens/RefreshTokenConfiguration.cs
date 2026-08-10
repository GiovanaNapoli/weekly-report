using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.RefreshTokens;

public class RefreshTokenConfiguration : AuditableEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("TB_REFRESH_TOKENS", schema: "dbo");

        builder.Property(rt => rt.UserId)
            .HasColumnName("FK_USER")
            .IsRequired();

        builder.Property(rt => rt.TokenHash)
            .HasColumnName("TX_TOKEN_HASH")
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

        builder.Property(rt => rt.ExpiresAt)
            .HasColumnName("DT_EXPIRES_AT")
            .IsRequired();

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);
    }
}
