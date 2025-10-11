using CarbonTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbonTrackerApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Edificio> Edificios { get; set; }
    public DbSet<MedidorEnergia> MedidoresEnergia { get; set; }
    public DbSet<MedicaoEnergia> MedicoesEnergia { get; set; }
    public DbSet<MetaCarbono> MetasCarbono { get; set; }
    public DbSet<FatorEmissao> FatoresEmissao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToUpper());
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToUpper());
            }
        }

        modelBuilder.Entity<MedicaoEnergia>()
            .Property(m => m.ConsumoValor)
            .HasColumnType("decimal(18,4)");

        modelBuilder.Entity<MetaCarbono>()
            .Property(m => m.ReducaoPercentual)
            .HasColumnType("decimal(5,4)");

        modelBuilder.Entity<FatorEmissao>()
            .Property(f => f.ValorEmissao)
            .HasColumnType("decimal(10,6)");

        modelBuilder.Entity<MedidorEnergia>()
            .HasIndex(m => m.NumeroSerie)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Username)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}