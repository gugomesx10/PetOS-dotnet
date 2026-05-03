using Microsoft.EntityFrameworkCore;
using PetOS.Models;

namespace PetOS.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();
    public DbSet<Rotina> Rotinas => Set<Rotina>();
    public DbSet<Alerta> Alertas => Set<Alerta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("TB_PET");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Vacina>(entity =>
        {
            entity.ToTable("TB_VACINA");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).ValueGeneratedOnAdd();

            entity.HasOne(v => v.Pet)
                .WithMany(p => p.Vacinas)
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Rotina>(entity =>
        {
            entity.ToTable("TB_ROTINA");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Property(r => r.Ativa).HasConversion<short>();

            entity.HasOne(r => r.Pet)
                .WithMany(p => p.Rotinas)
                .HasForeignKey(r => r.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.ToTable("TB_ALERTA");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();

            entity.HasOne(a => a.Pet)
                .WithMany(p => p.Alertas)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Rotina)
                .WithMany(r => r.Alertas)
                .HasForeignKey(a => a.RotinaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}
