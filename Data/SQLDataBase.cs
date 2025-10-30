using Microsoft.EntityFrameworkCore;

namespace Crewly.Data;

public static class DataBaseHandler
{
    public static void EnsureMigrated()
    {
        using var dbContext = new BotDbContext();
        dbContext.Database.Migrate();
    }
}

internal class BotDbContext : DbContext
{
    public DbSet<ExecutorData> Executors { get; set; }
    public DbSet<ClientData> Clients { get; set; }
    public DbSet<TaskData> Tasks { get; set; }
    
    private readonly string _connectionString = "Server=localhost;Database=Main;Integrated Security=true;TrustServerCertificate=True;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExecutorData>().ToTable("Executors").UseTpcMappingStrategy();
        modelBuilder.Entity<ClientData>().ToTable("Clients").UseTpcMappingStrategy();
        modelBuilder.Entity<TaskData>().ToTable("Tasks");    
        
        modelBuilder.Entity<ExecutorData>().HasKey(e => e.UserId);
        modelBuilder.Entity<ClientData>().HasKey(e => e.UserId);
        modelBuilder.Entity<TaskData>().HasKey(e => e.TaskId);
    }
}