using Microsoft.EntityFrameworkCore;
using PetOS.Models;

namespace PetOS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<RoutineRecord> RoutineRecords { get; set; }
    public DbSet<Alert>  Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pet>()
            .Property(p => p.Weight)
            .HasPrecision(10, 2);
    }
}