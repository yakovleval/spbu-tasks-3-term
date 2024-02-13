using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Server.Data;

public class HistoryDbContext : DbContext
{
    public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<AssemblyResult> Assemblies => Set<AssemblyResult>();
    public DbSet<ClassResult> Classes => Set<ClassResult>();
    public DbSet<MethodResult> Methods => Set<MethodResult>();
}
