using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PetOS.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseOracle(
            "User Id=rm555999;Password=250403;Data Source=oracle.fiap.com.br:1521/orcl");

        return new AppDbContext(optionsBuilder.Options);
    }
}

