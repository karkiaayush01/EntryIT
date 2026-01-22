using EntryIt.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace EntryIt.Data;
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Journal> Journals { get; set; } = null!;
    public DbSet<Mood> Moods { get; set; } = null!;
    public DbSet<Streak> StreakRecords { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Journal_Tag> Journal_Tags { get; set; } = null!;

    private readonly string _dbPath;

    public AppDbContext()
    {
        // Path to store SQLite DB on device
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = System.IO.Path.Combine(folder, "app.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}