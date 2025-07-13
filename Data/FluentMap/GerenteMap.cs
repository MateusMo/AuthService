using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.FluentMap;

public class GerenteMap : IEntityTypeConfiguration<Gerente>
{
    public void Configure(EntityTypeBuilder<Gerente> builder)
    {
        builder.ToTable("Gerente");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
        
        builder.Property(x => x.Nome)
            .IsRequired()
            .HasColumnName("Nome")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasColumnName("Email")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);
        
        builder.Property(x => x.Senha)
            .IsRequired()
            .HasColumnName("Senha")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(20);

    }
}