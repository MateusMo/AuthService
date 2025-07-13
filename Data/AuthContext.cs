using Data.FluentMap;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AuthContext : DbContext
{
    public DbSet<Gerente> Gerentes { get; set; }

    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GerenteMap());
    }
}